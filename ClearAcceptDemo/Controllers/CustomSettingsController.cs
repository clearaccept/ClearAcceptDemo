using ClearAcceptDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ClearAcceptDemo.Controllers
{
    public class CustomSettingsController : Controller
    {
        private readonly SettingsModel _settings;
        private readonly IMemoryCache _memoryCache;

        public CustomSettingsController(IOptions<SettingsModel> settings, IMemoryCache memoryCache)
        {
            _settings = settings.Value;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            var viewModel = _memoryCache.Get<CustomSettingsViewModel>("customSettings");
            if (viewModel == null)
            {
                viewModel = new CustomSettingsViewModel
                {
                    TransactCredentials = _settings.TransactCredentials,
                    PaymentRequest = _settings.PaymentRequest,
                    Identifiers = _settings.Identifiers
                };
            }
            if (viewModel.TransactCredentials.ClientSecret != null)
            {
                viewModel.TransactCredentials.ClientSecret = "################";
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Save(CustomSettingsViewModel model)
        {
            model.PaymentRequest.MerchantAccountId = model.Identifiers.MerchantAccountId;
            
            _memoryCache.Set("customSettings", model);
            if (model.TransactCredentials.ClientSecret != null)
            {
                model.TransactCredentials.ClientSecret = "################";
            }

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Reset()
        {
            _memoryCache.Remove("customSettings");
            var viewModel = new CustomSettingsViewModel
            {
                TransactCredentials = _settings.TransactCredentials,
                PaymentRequest = _settings.PaymentRequest,
                Identifiers = _settings.Identifiers
            };
            if (viewModel.TransactCredentials.ClientSecret != null)
            {
                viewModel.TransactCredentials.ClientSecret = "################";
            }

            return View("Index", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
