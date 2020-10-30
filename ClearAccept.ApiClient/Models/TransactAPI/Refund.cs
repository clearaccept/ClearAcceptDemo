namespace ClearAccept.ApiClient.Models
{
    public class RefundRequest
    {
        public string MerchantAccountId { get; set; }
        public int Amount { get; set; }
    }

    public class RefundResponse
    {
        public string PaymentRequestId { get; set; }
        public string TransactionId { get; set; }
        public string ReferenceId { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public int Voided { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PlatformId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantAccountId { get; set; }
        public string DeclineReason { get; set; }
        public string DeclinedBy { get; set; }
    }
}
