using Microsoft.AspNetCore.Html;

namespace ClearAcceptDemo.Models
{
    public class HomeViewModel
    {
        public string PaymentsLibraryUrl { get; set; }
        public HtmlString PaymentRequestId { get; set; }
        public HtmlString FieldToken { get; set; }
        public string Channel { get; set; }
        public string Error { get; set; }
    }
}
