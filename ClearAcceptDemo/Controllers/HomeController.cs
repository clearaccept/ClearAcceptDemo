using ClearAccept.ApiClient.Models;
using ClearAccept.ApiClient.Services;
using ClearAcceptDemo.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace ClearAcceptDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly SettingsModel _settings;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly TransactService _service;
        private readonly IMemoryCache _memoryCache;

        public HomeController(IOptions<SettingsModel> settings, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            var customSettings = _memoryCache.Get<CustomSettingsViewModel>("customSettings");

            _settings = new SettingsModel
            {
                AuthenticationEndpoint = settings.Value.AuthenticationEndpoint,
                HostedFieldsUrl = settings.Value.HostedFieldsUrl,
                TransactUrl = settings.Value.TransactUrl,
                TransactCredentials = customSettings?.TransactCredentials ?? settings.Value.TransactCredentials,
                PaymentRequest = customSettings?.PaymentRequest ?? settings.Value.PaymentRequest,
                Identifiers = customSettings?.Identifiers ?? settings.Value.Identifiers
            };

            _serviceConfiguration = new ServiceConfiguration
            {
                Authentication = new AuthConfig
                {
                    URL = _settings.AuthenticationEndpoint,
                    Credentials = _settings.TransactCredentials,
                    Resource = "transact"
                },
                Host = _settings.TransactUrl,
                PlatformId = _settings.Identifiers.PlatformId,
                MerchantId = _settings.Identifiers.MerchantId
            };
            _service = new TransactService(_serviceConfiguration, memoryCache);
        }

        public IActionResult Index()
        {
            var viewModel = new HomeViewModel();

            try
            {
                var countries = _memoryCache.Get<List<IsoCountryModel>>("countries");
                var currencies = _memoryCache.Get<List<CurrencyModel>>("currencies");

                // Build PaymentRequest object
                var paymentRequest = _settings.PaymentRequest;
                paymentRequest.MerchantAccountId = _settings.Identifiers.MerchantAccountId;

                // Call POST /payment-requests
                var paymentRequestResponse = _service.PostPaymentRequest(paymentRequest);

                // Call POST /field-tokens
                var fieldToken = _service.PostFieldToken(new FieldTokenRequest
                {
                    PaymentRequestId = paymentRequestResponse.PaymentRequestId
                });

                // Build view
                viewModel = new HomeViewModel
                {
                    PaymentsLibraryUrl = _settings.HostedFieldsUrl,          // Set Hosted Fields' script source
                    FieldToken = fieldToken.FieldToken,                      // Pass the fieldToken to the view
                    IsoCountries = countries,
                    Currencies = currencies,
                    SavedCards = _memoryCache.Get<List<SavedCard>>("savedCards")?
                                             .Where(i => i.CustomerReference == _settings.PaymentRequest.PlatformReferences.CustomerReference)?
                                             .ToList()
                                 ?? new List<SavedCard>(),
                    Currency = paymentRequest.Currency,
                    Amount = paymentRequest.Amount.GetValueOrDefault(),
                    Channel = paymentRequest.Channel.GetValueOrDefault(),
                    CustomerInfo = paymentRequest.CustomerInfo,
                    PaymentRequestId = paymentRequestResponse.PaymentRequestId
                };

                // DEMO PURPOSES ONLY
                // Build and set Request/Response to be displayed in View
                var requestString = "Create PaymentRequest" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "POST /payment-requests" +
                    Environment.NewLine +
                    $"Host: {_serviceConfiguration.Host}" +
                    Environment.NewLine +
                    Environment.NewLine +
                    JsonSerializer.Serialize(paymentRequest, new JsonSerializerOptions { WriteIndented = true }) +
                    Environment.NewLine +
                    Environment.NewLine +
                    "----------------------------------------------" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Create FieldToken" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "POST /field-tokens" +
                    Environment.NewLine +
                    $"Host: {_serviceConfiguration.Host}" +
                    Environment.NewLine +
                    Environment.NewLine +
                    JsonSerializer.Serialize(fieldToken, new JsonSerializerOptions { WriteIndented = true });
                var responseString = "Create PaymentRequest " +
                    Environment.NewLine +
                    Environment.NewLine +
                    "201 Created" +
                    Environment.NewLine +
                    Environment.NewLine +
                    JsonSerializer.Serialize(paymentRequestResponse, new JsonSerializerOptions { WriteIndented = true }) +
                    Environment.NewLine +
                    Environment.NewLine +
                    "----------------------------------------------" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Create FieldToken" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "201 Created" +
                    Environment.NewLine +
                    Environment.NewLine +
                    JsonSerializer.Serialize(viewModel.FieldToken, new JsonSerializerOptions { WriteIndented = true });

                viewModel.Request = new HtmlString(requestString);
                viewModel.Response = new HtmlString(responseString);
            }
            catch (Exception e)
            {
                // DEMO PURPOSES ONLY
                viewModel.Error = "Backend exception:" + Environment.NewLine + e.Message;
            }

            return View(viewModel);
        }

        // Method to update a PaymentRequest with the selected PermanentToken
        [HttpPost]
        public IActionResult Update([FromQuery] string permanentToken, [FromQuery] string requestId)
        {
            var paymentRequest = _settings.PaymentRequest;
            paymentRequest.PaymentMethod = new PaymentMethod {
                Token = permanentToken
            };
            paymentRequest.MerchantAccountId = _settings.Identifiers.MerchantAccountId;

            var result = _service.PutPaymentRequest(requestId, paymentRequest);

            return Ok(result);
        }

        // Method to confirm a PaymentRequest
        [HttpPost]
        public IActionResult Process(HomeViewModel model)
        {
            var viewModel = new ProcessViewModel();
            var paymentRequest = new PaymentRequest
            {
                MerchantAccountId = _settings.Identifiers.MerchantAccountId,
                CustomerInfo = model.CustomerInfo,
                PaymentMethod = model.PaymentMethod
            };

            try
            {
                var result = new PaymentRequestResponse();
                try
                {
                    // Call POST /payment-requests/{paymentRequestId}/confirm
                    result = _service.ConfirmPaymentRequest(model.PaymentRequestId, paymentRequest);
                }
                // Handle timeouts
                catch (TimeoutException e)
                {
                    // Call GET /payment-requests/{paymentRequestId} to check if the PaymentRequest was confirmed
                    result = _service.GetPaymentRequest(model.PaymentRequestId);

                    // Call POST /payment-requests/{paymentRequestId}/confirm again if the PaymentRequest was not found to be confirmed
                    if (result.Status != Enums.PaymentRequestStatus.confirmed)
                    {
                        result = _service.ConfirmPaymentRequest(model.PaymentRequestId, paymentRequest);
                    }
                }

                // Store PermanentToken if the following conditions are met:
                // the customer chose to save the card
                // the payment was approved
                // the token in the response starts with "cardperm_"
                var approvedAuth = result.Transactions?.FirstOrDefault(i => i.Type == Enums.TransactionType.Authorisation && i.Status == Enums.TransactionStatus.Approved);

                if (paymentRequest.PaymentMethod.Persist &&
                    approvedAuth != null &&
                    result.Token.Substring(0, 9) == "cardperm_")
                {
                    var savedCards = _memoryCache.Get<List<SavedCard>>("savedCards") ?? new List<SavedCard>();

                    // Do not store a duplicate card
                    if (!savedCards.Any(i => i.Token == result.Token && i.CustomerReference == result.PlatformReferences.CustomerReference))
                    {
                        var newSavedCard = new SavedCard
                        {
                            Token = result.Token,
                            CardNumber = approvedAuth.CardNumber,
                            ExpiryDate = approvedAuth.ExpirationDate,
                            CardScheme = approvedAuth.CardScheme,
                            CustomerReference = result.PlatformReferences.CustomerReference
                        };
                        savedCards.Add(newSavedCard);
                        _memoryCache.Set("savedCards", savedCards);
                    }
                }

                // DEMO PURPOSES ONLY
                // Build and set Request/Response to be displayed in View
                var requestString = $"{_serviceConfiguration.Host}/payment-requests/{model.PaymentRequestId}/confirm" +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    JsonSerializer.Serialize(paymentRequest, new JsonSerializerOptions { WriteIndented = true });
                var responseString = JsonSerializer.Serialize(result,
                                    new JsonSerializerOptions { WriteIndented = true });

                viewModel.Request = new HtmlString(requestString);
                viewModel.Response = new HtmlString(responseString);
                viewModel.Result = new HtmlString($"STATUS: {result.Transactions?.FirstOrDefault(i => i.Type == Enums.TransactionType.Authorisation)?.Status.ToString() ?? "Error encountered."}");
            }
            catch (Exception e)
            {
                // DEMO PURPOSES ONLY
                viewModel.Response = new HtmlString(e.ToString());
            }

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
