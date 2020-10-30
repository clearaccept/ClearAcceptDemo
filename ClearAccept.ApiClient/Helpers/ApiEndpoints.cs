namespace ClearAccept.ApiClient.Helpers
{
    public static class ApiEndpoints
    {
        public static string EnvironmentName { get; set; }
        public static string IdentityEndpoint => "/oauth2/token";
        public static string PaymentRequestsEndpoint => "/payment-requests";
        public static string FieldTokensEndpoint => "/field-tokens";
    }
}
