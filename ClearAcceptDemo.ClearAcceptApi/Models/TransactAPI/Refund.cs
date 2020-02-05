namespace ClearAcceptDemo.ClearAcceptApi.Models
{
    public class RefundRequest
    {
        public string RelatedTransactionId { get; set; }
        public int RefundAmount { get; set; }
        public string MerchantAccountId { get; set; }
    }
}
