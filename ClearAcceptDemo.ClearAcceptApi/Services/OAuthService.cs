using System;
using System.Text;
using ClearAcceptDemo.ClearAcceptApi.Helpers;
using ClearAcceptDemo.ClearAcceptApi.Models;
using Newtonsoft.Json;
using RestSharp;

namespace ClearAcceptDemo.ClearAcceptApi.Services
{
    public class OAuthService
    {
        private readonly AuthConfig _authentication;
        private readonly RestClient _client;

        public OAuthService(AuthConfig authentication)
        {
            _authentication = authentication;
            _client = RestApiHelper.CreateAuthenticatedRestClient($"{authentication.URL}");
        }

        public AuthToken GetToken()
        {
            var request = new RestRequest(Method.POST);
            var base64 =
    Convert.ToBase64String(Encoding.ASCII.GetBytes(_authentication.Credentials.ClientId + ":" + _authentication.Credentials.ClientSecret));

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", $"Basic {base64}");
            request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials", ParameterType.RequestBody);

            var response = RestApiHelper.ExecuteRequest(_client, request);
            if (response.IsError)
            {
                throw new Exception(response.ErrorMessage);
            }

            try
            {
                var result = JsonConvert.DeserializeObject<AuthToken>(response.Content);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred while parsing the response from '{_client.BaseUrl}'",
                    ex);
            }
        }
    }
}