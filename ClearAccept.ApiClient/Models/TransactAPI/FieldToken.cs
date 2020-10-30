namespace ClearAccept.ApiClient.Models
{
    public class FieldTokenRequest
    {
        public string PaymentRequestId { get; set; }
    }

    public class FieldTokenResponse
    {
        public string PaymentRequestId { get; set; }
        public string FieldToken { get; set; }
    }    
}
