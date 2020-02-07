using System;

namespace ClearAccept.ApiClient.Models
{
    public class RefundModel
    {
        public string PaymentRequestId { get; set; }
        public string TransactionId { get; set; }
        public string ReferenceId { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string PlatformId { get; set; }
        public string MerchantId { get; set; }
        public string MerchantAccountId { get; set; }
        public string DeclineReason { get; set; }
        public string DeclinedBy { get; set; }
        public string SettlementId { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string RetrievalReferenceNumber { get; set; }
        public string AcquirerTransactionId { get; set; }
    }
}
