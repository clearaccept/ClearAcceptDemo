using Newtonsoft.Json;

namespace ClearAccept.ApiClient.Models
{
    public class AuthConfig
    {
        public string URL { get; set; }
        public Credentials Credentials { get; set; }
        public string Resource { get; set; }
    }

    public class AuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class Credentials
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}