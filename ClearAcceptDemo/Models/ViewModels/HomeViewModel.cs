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
        public string PaymentRequestId { get; set; }        
        public int Amount { get; set; }
        public string Currency { get; set; }
        public Enums.Channel Channel { get; set; }        
        public PaymentMethod PaymentMethod { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public string FieldToken { get; set; }
        public IList<IsoCountryModel> IsoCountries { get; set; }
        public IList<CurrencyModel> Currencies { get; set; }
        public string Error { get; set; }
        public IList<SavedCard> SavedCards { get; set; }
    }
}
