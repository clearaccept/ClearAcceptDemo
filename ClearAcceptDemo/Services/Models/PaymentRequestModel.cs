
namespace ClearAcceptDemo.Services.Models
{
    public class PaymentRequestModel
    {
        public PaymentMethodModel PaymentMethod { get; set; }
        public float Amount { get; set; }
        public string Currency { get; set; }
        public string Channel { get; set; }
        public string MerchantAccountId { get; set; }
    }
}

