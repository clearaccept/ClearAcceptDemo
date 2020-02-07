using ClearAccept.ApiClient.Models;

namespace ClearAcceptDemo.Models
{
    public class SettingsModel
    {
        public string AuthenticationEndpoint { get; set; }
        public string HostedFieldsUrl { get; set; }        
        public string TransactUrl { get; set; }
        public Credentials TransactCredentials { get; set; }
        public PaymentRequestModel PaymentRequest { get; set; }
        public Identifiers Identifiers { get; set; }
    }
}
