namespace ClearAccept.ApiClient.Helpers
{
    public static class ApiEndpoints
    {
        public static string EnvironmentName { get; set; }
        public static string IdentityEndpoint => "/oauth2/token";
        public static string PaymentRequestsEndpoint => "/payment-requests";
        public static string FieldTokensEndpoint => "/field-tokens";
        public static string TemporaryTokensEndpoint => "/temporary-tokens";
        public static string PermanentTokensEndpoint => "/permanent-tokens";
        public static string PlatformsEndpoint => "/platforms";
        public static string MerchantsEndpoint => "/merchants";
        public static string AccountsEndpoint => "/merchant-accounts";
        public static string AuthorisationsEndpoint => "/authorisations";
        public static string TransactionsEndpoint => "/settlements";
    }
}
