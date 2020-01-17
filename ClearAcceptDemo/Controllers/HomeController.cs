using ClearAcceptDemo.Models;
using ClearAcceptDemo.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClearAcceptDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly PaymentsService _paymentsService;
        private readonly SettingsModel _settings;

        public HomeController(IOptions<SettingsModel> settings)
        {
            _settings = settings.Value;
            _paymentsService = new PaymentsService();
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            try
            {
                viewModel.PaymentsLibraryUrl = _settings.HostedFieldsUrl;
                viewModel.Channel = _settings.Payment.Channel;

                var accessToken =
                    await _paymentsService.GetAccessToken(_settings.AuthenticationEndpoint,
                        _settings.PaymentCredentials);

                var paymentRequest = await _paymentsService.CreatePaymentRequest(_settings.PaymentsEndpoint,
                    accessToken, _settings.Payment, _settings.Platform);
                var paymentRequestId = paymentRequest.PaymentRequestId;

                viewModel.PaymentRequestId = new HtmlString(paymentRequestId);

                var fieldToken = await _paymentsService.CreateFieldToken(_settings.FieldTokensEndpoint, paymentRequestId,
                    accessToken, _settings.Platform);
                viewModel.FieldToken = new HtmlString(fieldToken);
            }
            catch (Exception e)
            {
                viewModel.Error = "Backend exception:" + Environment.NewLine + e.Message;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Process(string temporaryToken, string requestId)
        {
            var viewModel = new ProcessViewModel();
            try
            {
                var accessToken =
                    await _paymentsService.GetAccessToken(_settings.AuthenticationEndpoint,
                        _settings.PaymentCredentials);
                var result = await _paymentsService.ConfirmPaymentRequest(_settings.PaymentsEndpoint, accessToken,
                     temporaryToken, requestId, _settings.Platform, _settings.Payment);
                viewModel.Request = new HtmlString(result.Item1);
                viewModel.Response = new HtmlString(result.Item2);
                viewModel.Result = new HtmlString(_paymentsService.GetStatusFromPaymentRequest(result.Item2));
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
