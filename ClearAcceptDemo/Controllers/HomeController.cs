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

            // Set the MerchantAccountId on the payment-request body
            _settings.PaymentRequest.MerchantAccountId = _settings.Identifiers.MerchantAccountId;

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
            var countries = _memoryCache.Get<List<IsoCountryModel>>("countries");
            var currencies = _memoryCache.Get<List<CurrencyModel>>("currencies");
            var viewModel = new HomeViewModel
            {
                IsoCountries = countries,
                Currencies = currencies,
                SavedCards = _memoryCache.Get<List<SavedCard>>("savedCards")?.Where(i => i.CustomerReference == _settings.PaymentRequest.PlatformReferences.CustomerReference)?.ToList() ?? new List<SavedCard>()
            };

            try
            {
                // Set Hosted Fields' script source
                viewModel.PaymentsLibraryUrl = _settings.HostedFieldsUrl;
               
                // Call POST /payment-requests
                viewModel.PaymentRequest = _service.PostPaymentRequest(_settings.PaymentRequest);

                // Call POST /field-tokens
                var fieldToken = new FieldTokenModel
                {
                    PaymentRequestId = viewModel.PaymentRequest.PaymentRequestId
                };
                viewModel.FieldToken = _service.PostFieldToken(fieldToken);

                // Build and set Request/Response to be displayed in View
                var requestString = "Create PaymentRequest" +
                    Environment.NewLine +
                    Environment.NewLine +
                    "POST /payment-requests" +
                    Environment.NewLine +
                    $"Host: {_serviceConfiguration.Host}" +
                    Environment.NewLine +
                    Environment.NewLine +
                    JsonSerializer.Serialize(_settings.PaymentRequest, new JsonSerializerOptions { WriteIndented = true }) +
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
                    JsonSerializer.Serialize(viewModel.PaymentRequest, new JsonSerializerOptions { WriteIndented = true }) +
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
                viewModel.Error = "Backend exception:" + Environment.NewLine + e.Message;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Process(HomeViewModel model)
        {
            var viewModel = new ProcessViewModel();
            var paymentRequest = model.PaymentRequest;

            try
            {
                // Call POST /payment-requests/{paymentRequestId}/confirm
                var result = _service.ConfirmPaymentRequest(paymentRequest);

                // Store Permanent Token
                var approvedAuth = result.Transactions?.FirstOrDefault(i => i.Type == "Authorisation" && (i.Status == "Approved" || i.Status == "Settled"));
                if (approvedAuth != null && result.Token.Substring(0, 8).Contains("perm"))
                {
                    var savedCards = _memoryCache.Get<List<SavedCard>>("savedCards") ?? new List<SavedCard>();

                    if (!savedCards.Any(i => i.Token == result.Token && i.CustomerReference == result.PlatformReferences.CustomerReference))
                    {
                        var newSavedCard = new SavedCard
                        {
                            Token = result.Token,
                            CardNumber = approvedAuth.CardNumber,
                            ExpiryDate = approvedAuth.ExpirationDate,
                            CustomerReference = result.PlatformReferences.CustomerReference
                        };
                        savedCards.Add(newSavedCard);
                        _memoryCache.Set("savedCards", savedCards);
                    }
                }

                // Build and set Request/Response to be displayed in View
                var requestString = $"{_serviceConfiguration.Host}/payment-requests/{paymentRequest.PaymentRequestId}/confirm" +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    JsonSerializer.Serialize(paymentRequest, new JsonSerializerOptions { WriteIndented = true });
                var responseString = JsonSerializer.Serialize(result,
                                    new JsonSerializerOptions { WriteIndented = true });

                viewModel.Request = new HtmlString(requestString);
                viewModel.Response = new HtmlString(responseString);
                viewModel.Result = new HtmlString($"STATUS: {result.Transactions?.FirstOrDefault(i => i.Type == "Authorisation")?.Status ?? "Error encountered."}");
            }
            catch (Exception e)
            {
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
