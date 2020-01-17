namespace ClearAcceptDemo.Services.Models
{
    public class PaymentMethodModel
    {
        public string Token { get; set; }
        public bool Persist { get; set; }
        public string Cvv { get; set; }
    }
}

