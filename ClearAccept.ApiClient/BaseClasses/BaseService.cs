using System;
using ClearAccept.ApiClient.Helpers;
using ClearAccept.ApiClient.Models;
using ClearAccept.ApiClient.Services;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;

namespace ClearAccept.ApiClient.BaseClasses
{
    public abstract class BaseService : IService
    {
        protected AuthToken AuthToken { get; set; }
        protected readonly ServiceConfiguration _configuration;

        private readonly IMemoryCache _memoryCache;

        protected BaseService(ServiceConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;

            if (_configuration.Authentication.Resource == null) throw new Exception("Missing Authentication.Resource.");

            // Retrieve object containing access_token from memory
            var authToken = _memoryCache.Get<AuthToken>($"cc_auth_{_configuration.Authentication.Resource}");
            if (authToken != null)
            {
                AuthToken = authToken;
            }
            else
            {
                Authenticate();
            }
        }

        public void Authenticate()
        {
            var oAuthService = new OAuthService(_configuration.Authentication);
            AuthToken = oAuthService.GetToken();
            if (AuthToken != null)
            {
                // Calculate access_token expiry date
                var expiresOn = DateTime.UtcNow.AddSeconds(AuthToken.ExpiresIn);
                var dateTimeOffset = new DateTimeOffset(expiresOn);

                // Store object containing access_token in memory and set to expire once access_token expires
                _memoryCache.Set($"cc_auth_{_configuration.Authentication.Resource}", AuthToken, dateTimeOffset);
            }
        }

        // Generic GET method for ClearAccept endpoints
        public TOut Get<TOut>(string url)
        {
            var client = RestApiHelper.CreateAuthenticatedRestClient(url);
            var request = new RestRequest { Method = Method.GET };
            AddClearAcceptHeaders(request);

            var response = RestApiHelper.ExecuteRequest(client, request);
            HandleApiException(response);

            try
            {
                var result = JsonConvert.DeserializeObject<TOut>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while parsing the response from '{client.BaseUrl}'",
                    ex);
            }
        }

        // Generic POST method for ClearAccept endpoints
        public TOut Post<TIn, TOut>(string url, TIn requestBody)
        {
            var client = RestApiHelper.CreateAuthenticatedRestClient(url);
            var request = new RestRequest { Method = Method.POST, RequestFormat = DataFormat.Json };
            AddClearAcceptHeaders(request);
            request.AddJsonBody(requestBody);

            var response = RestApiHelper.ExecuteRequest(client, request);
            HandleApiException(response);

            try
            {
                var result = JsonConvert.DeserializeObject<TOut>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while parsing the response from '{client.BaseUrl}'",
                    ex);
            }
        }

        // Generic PUT method for ClearAccept endpoints
        public TOut Put<TIn, TOut>(string url, TIn requestBody)
        {
            var client = RestApiHelper.CreateAuthenticatedRestClient(url);
            var request = new RestRequest { Method = Method.PUT, RequestFormat = DataFormat.Json };
            AddClearAcceptHeaders(request);
            request.AddJsonBody(requestBody);

            var response = RestApiHelper.ExecuteRequest(client, request);
            HandleApiException(response);

            try
            {
                var result = JsonConvert.DeserializeObject<TOut>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while parsing the response from '{client.BaseUrl}'",
                    ex);
            }
        }

        // Generic DELETE method for ClearAccept endpoints
        public System.Net.HttpStatusCode Delete(string url)
        {
            var client = RestApiHelper.CreateAuthenticatedRestClient(url);
            var request = new RestRequest { Method = Method.DELETE };
            AddClearAcceptHeaders(request);

            var response = RestApiHelper.ExecuteRequest(client, request);
            HandleApiException(response);

            return response.StatusCode;
        }

        private void AddClearAcceptHeaders(RestRequest request)
        {
            request.AddHeader("Authorization", $"Bearer {AuthToken.AccessToken}");
            request.AddHeader("PlatformId", _configuration.PlatformId);
            request.AddHeader("MerchantId", _configuration.MerchantId);
        }

        private void HandleApiException(ApiResponse response)
        {
            if (response.IsError)
            {
                // Throw an ApplicationException if a BadRequest was returned
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new ApplicationException($"ClearAccept Api BadRequest.", new Exception(response.ErrorMessage));
                }

                // Throw a TimeoutException if a timeout occured
                if (response.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                response.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
                {
                    throw new TimeoutException(response.ErrorMessage);
                }

                throw new Exception(response.ErrorMessage);
            }
        }
    }
}
