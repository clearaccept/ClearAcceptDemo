using System.Collections.Generic;
using ClearAccept.ApiClient.Models;
using Microsoft.AspNetCore.Html;

namespace ClearAcceptDemo.Models
{
    public class HomeViewModel
    {
        public HtmlString Request { get; set; }
        public HtmlString Response { get; set; }
        public string PaymentsLibraryUrl { get; set; }
        public PaymentRequestModel PaymentRequest { get; set; }
        public FieldTokenModel FieldToken { get; set; }
        public IList<IsoCountryModel> IsoCountries { get; set; }
        public IList<CurrencyModel> Currencies { get; set; }
        public string Error { get; set; }
        public IList<SavedCard> SavedCards { get; set; }
    }
}
