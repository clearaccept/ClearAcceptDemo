namespace ClearAccept.ApiClient.Models
{
    public class Enums
    {
        public enum Channel {
            ECOM,
            MO,
            TO
        }

        public enum PaymentRequestStatus {
            missing_token,
            pending_confirm,
            confirmed
        }

        public enum TransactionStatus {
            Pending,
            Approved,
            Declined
        }

        public enum TransactionType {
            Authorisation,
            Capture,
            Refund,
            Void
        }        
    }
}
