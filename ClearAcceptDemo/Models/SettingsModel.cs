namespace ClearAcceptDemo.Models
{
    public class SettingsModel
    {
        public string AuthenticationEndpoint { get; set; }
        public string HostedFieldsUrl { get; set; }        
        public string PaymentsEndpoint { get; set; }
        public string FieldTokensEndpoint { get; set; }
        public CredentialsModel PaymentCredentials { get; set; }
        public PaymentSettings Payment { get; set; }
        public PlatformSettings Platform { get; set; }
    }
}
