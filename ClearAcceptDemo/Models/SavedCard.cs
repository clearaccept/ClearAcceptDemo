namespace ClearAcceptDemo.Models
{
    public class SavedCard
    {
        public string Token {get;set;}
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CardScheme { get; set; }
        public string CustomerReference { get; set; }
    }
}
