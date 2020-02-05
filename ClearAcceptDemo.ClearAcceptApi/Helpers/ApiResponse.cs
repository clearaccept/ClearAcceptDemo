namespace ClearAcceptDemo.ClearAcceptApi.Helpers
{
    public class ApiResponse
    {
        public string Content { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
    }
}