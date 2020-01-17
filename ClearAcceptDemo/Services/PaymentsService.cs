using ClearAcceptDemo.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClearAcceptDemo.Services.Models;

namespace ClearAcceptDemo.Services
{
    public class PaymentsService
    {
        public async Task<string> GetAccessToken(string authenticationEndpoint, CredentialsModel credentials)
        {
            using var client = new HttpClient();
            var formContent = new Dictionary<string, string>
                {{"grant_type", "client_credentials"}, {"scope", credentials.Scope}};

            var authenticationHeader =
                Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials.ClientId + ":" + credentials.ClientSecret));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationHeader);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var postTask = await client.PostAsync(authenticationEndpoint, new FormUrlEncodedContent(formContent));
            var response = await postTask.Content.ReadAsStringAsync();
            try
            {
                var json = JsonDocument.Parse(response);

                if (json.RootElement.TryGetProperty("access_token", out var accessToken))
                    return accessToken.GetString();
                return "";
            }
            catch (Exception)
            {
                throw new Exception(response);
            }
        }
        public async Task<String> CreateFieldToken(string fieldsTokensEndpoint, string paymentRequestId,
            string accessToken, PlatformSettings platform)
        {
            using var client = new HttpClient();
            AddAuthenticationHeaders(client.DefaultRequestHeaders, accessToken, platform);

            var requestBody = new
            {
                PaymentRequestId = paymentRequestId
            };

            var stringContent =
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var postTask = await client.PostAsync(fieldsTokensEndpoint, stringContent);
            var response = await postTask.Content.ReadAsStringAsync();
            try
            {
                var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("fieldNonce", out var fieldToken))
                {
                    return fieldToken.GetString();
                }

                throw new Exception(response);
            }
            catch (Exception)
            {

                throw new Exception(response);
            }
        }

        public async Task<PaymentResponseModel> CreatePaymentRequest(string paymentsEndpoint,
            string accessToken, PaymentSettings payment,
            PlatformSettings platform)
        {
            using var client = new HttpClient();
            AddAuthenticationHeaders(client.DefaultRequestHeaders, accessToken, platform);

            var requestBody = new PaymentRequestModel
            {
                Amount = payment.Amount,
                Currency = payment.Currency,
                Channel = payment.Channel,
                MerchantAccountId = platform.MerchantAccountId
            };

            var stringContent =
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var postTask = await client.PostAsync(paymentsEndpoint, stringContent);
            var response = await postTask.Content.ReadAsStringAsync();
            try
            {
                var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("paymentRequestId", out var paymentRequestId) &&
                    json.RootElement.TryGetProperty("channel", out var channel))
                {
                    return new PaymentResponseModel()
                    {
                        Channel = channel.GetString(),
                        PaymentRequestId = paymentRequestId.GetString()
                    };
                }

                throw new Exception(response);
            }
            catch (Exception)
            {

                throw new Exception(response);
            }
        }

        public async Task<Tuple<string, string>> ConfirmPaymentRequest(string paymentsEndpoint, string accessToken, string temporaryToken, string requestId, PlatformSettings platform,
            PaymentSettings payment)
        {
            var url = paymentsEndpoint + "/" + requestId + "/confirm";
            using var client = new HttpClient();
            AddAuthenticationHeaders(client.DefaultRequestHeaders, accessToken, platform);

            var requestBody = new PaymentRequestModel
            {
                PaymentMethod = new PaymentMethodModel
                {
                    Token = temporaryToken
                },
                MerchantAccountId = platform.MerchantAccountId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Channel = payment.Channel,
            };

            var stringContent =
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var postTask = await client.PostAsync(url, stringContent);
            var response = await postTask.Content.ReadAsStringAsync();

            response = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(response),
                new JsonSerializerOptions { WriteIndented = true });

            var request = url + Environment.NewLine +
                          JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { WriteIndented = true });
            return new Tuple<string, string>(request, response);
        }

        public string GetStatusFromPaymentRequest(string paymentRequestJson)
        {
            var json = JsonDocument.Parse(paymentRequestJson);

            if (json.RootElement.TryGetProperty("transactions", out var transactions))
            {
                var transactionsLength = transactions.GetArrayLength();
                if (transactionsLength == 0)
                {
                    return "ERROR: Empty transactions list";
                }

                var lastTransaction = transactions[transactionsLength - 1];
                var transactionStatus = lastTransaction.GetProperty("status").GetString();
                return "STATUS: " + transactionStatus;
            }


            if (json.RootElement.TryGetProperty("errors", out var errors))
            {
                var errorsLength = errors.GetArrayLength();
                if (errorsLength == 0)
                {
                    return "ERROR: Empty errors list";
                }
                var firstError = errors[0];
                var errorMessage = firstError.GetProperty("message").GetString();
                return "ERROR: " + errorMessage;
            }

            if (json.RootElement.TryGetProperty("message", out var message))
            {
                return "ERROR: " + message.GetString();
            }

            return "ERROR: Unknown response from payment request";
        }

        private void AddAuthenticationHeaders(HttpRequestHeaders headers, string accessToken, PlatformSettings platform)
        {
            headers.Add("merchantId", platform.MerchantId);
            headers.Add("platformId", platform.PlatformId);
            headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
