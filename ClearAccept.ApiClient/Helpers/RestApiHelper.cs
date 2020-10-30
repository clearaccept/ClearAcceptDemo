using System;
using System.Linq;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace ClearAccept.ApiClient.Helpers
{
    public static class RestApiHelper
    {
        public static RestClient CreateAuthenticatedRestClient(string url)
        {
            var restClient = new RestClient(url);

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            restClient.UseNewtonsoftJson(jsonSettings);
            return restClient;
        }

        public static ApiResponse ExecuteRequest(RestClient restClient, RestRequest restRequest)
        {
            var executionCount = 0;
            const int retryCount = 3;

            var errorMessage = string.Empty;
            var result = new ApiResponse { IsError = true };

            while (executionCount < retryCount)
            {
                try
                {
                    var response = restClient.Execute(restRequest);
                    result.StatusCode = response.StatusCode;

                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created ||
                        response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        result.Content = response.Content;
                        result.IsError = false;
                        result.ErrorMessage = string.Empty;

                        return result;
                    }

                    // If we get this far, there is an error
                    errorMessage =
                        $"Error Executing {restClient.BaseUrl}, Parameters: {GetRequestBody(restRequest)}, Status Code: {response.StatusCode}, Status Description: {response.StatusDescription}";

                    // If the HTTP status code is in the 400 range, attempt to include actual error messge
                    if ((int)response.StatusCode < 400 || (int)response.StatusCode >= 500) continue;
                    var errorResult = response.Content;
                    errorMessage += $", Message: {errorResult}";
                    break;
                }
                catch (Exception ex)
                {
                    errorMessage =
                        $"An unexpected exception occurred while attempting to execute '{restClient.BaseUrl}'.  Error: {ex.Message}";
                }
                finally
                {
                    executionCount++;
                }
            }

            // If we get this far we must have reached the retry count, return the full error
            if (result.IsError)
            {
                result.ErrorMessage = errorMessage;
            }
            return result;
        }

        private static string GetRequestBody(RestRequest restRequest)
        {
            var requestBody = restRequest.Parameters.FirstOrDefault(i => i.Type == ParameterType.RequestBody);
            return requestBody?.Value.ToString() ?? JsonConvert.SerializeObject(restRequest.Parameters);
        }
    }
}