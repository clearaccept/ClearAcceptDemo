using System.Collections.Generic;

namespace ClearAccept.ApiClient.Models
{
    public class PaymentRequest
    {
        public bool? Confirm { get; set; }
        public bool? Capture { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int? Amount { get; set; }
        public string Currency { get; set; }
        public Enums.Channel? Channel { get; set; }
        public string MerchantAccountId { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public PlatformReferences PlatformReferences { get; set; }
        public MerchantReferences MerchantReferences { get; set; }
    }

    public class PaymentRequestResponse
    {
        public string Token { get; set; }
        public string CvvToken { get; set; }
        public string MerchantAccountId { get; set; }
        public string MerchantId { get; set; }
        public string PlatformId { get; set; }        
        public string PaymentRequestId { get; set; }
        public int Amount { get; set; }
        public bool? Capture { get; set; }
        public string Currency { get; set; }
        public Enums.Channel Channel { get; set; }
        public string ProcessorName { get; set; }
        public Enums.PaymentRequestStatus Status { get; set; }
        public CustomerInfo CustomerInfo { get; set; }
        public PlatformReferences PlatformReferences { get; set; }
        public MerchantReferences MerchantReferences { get; set; }
        public Threeds Threeds { get; set; }
        public List<Transaction> Transactions { get; set; }
    }    

    public class PaymentMethod
    {
        public string Token { get; set; }
        public bool Persist { get; set; }
        public string CvvToken { get; set; }
    }

    public class CustomerInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public BillingAddress BillingAddress { get; set; }
    }

    public class BillingAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
    }

    public class PlatformReferences
    {
        public string CustomerReference { get; set; }
        public string InvoiceReference { get; set; }
        public string TransactionReference { get; set; }
    }

    public class MerchantReferences
    {
        public string CustomerReference { get; set; }
        public string InvoiceReference { get; set; }
        public string TransactionReference { get; set; }
    }

    public class Threeds
    {
        public string Eci { get; set; }
        public string ThreedsStatus { get; set; }
        public string ThreedsStatusReason { get; set; }
        public string Enrolled { get; set; }
        public string ThreedsId { get; set; }
        public string Version { get; set; }
        public string LiabilityShiftStatus { get; set; }
    }

    public class Transaction
    {
        public string TransactionId { get; set; }
        public string ReferenceId { get; set; }
        public Enums.TransactionType Type { get; set; }
        public string Token { get; set; }
        public string CvvToken { get; set; }
        public string AuthCode { get; set; }
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardType { get; set; }
        public string CardScheme { get; set; }
        public string CardBrand { get; set; }
        public string CardCurrency { get; set; }
        public string IssuerCountry { get; set; }
        public string IssuerName { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public int Voided { get; set; }
        public int Refunded { get; set; }
        public int Captured { get; set; }
        public string Currency { get; set; }
        public Enums.TransactionStatus Status { get; set; }
        public string AvsMatch { get; set; }
        public string CvvMatch { get; set; }
        public Enums.Channel Channel { get; set; }
        public string DeclinedBy { get; set; }
        public string DeclineReason { get; set; }
        public Threeds Threeds { get; set; }
    }
}
