using ClearAccept.ApiClient.Models;

namespace ClearAcceptDemo.Models
{
    public class CustomSettingsViewModel
    {
        public Credentials TransactCredentials { get; set; }
        public PaymentRequestModel PaymentRequest { get; set; }
        public Identifiers Identifiers { get; set; }
    }
}
