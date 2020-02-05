namespace ClearAcceptDemo.ClearAcceptApi.Models
{
    public class ServiceConfiguration
    {
        public string Host { get; set; }
        public AuthConfig Authentication { get; set; }
        public string PlatformId { get; set; }
        public string MerchantId { get; set; }
    }
}