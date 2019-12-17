using DdpClient.EJson;
using Newtonsoft.Json;

namespace DdpClient.Models
{
    internal class BasicLoginModel<T>
    {
        [JsonProperty("user")]
        public T User { get; set; }

        [JsonProperty("password")]
        public PasswordModel Password { get; set; }
    }

    internal class BasicTokenModel
    {
        [JsonProperty("resume")]
        public string Resume { get; set; }
    }

    internal class EmailUser
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }

    internal class UsernameUser
    {
        [JsonProperty("username")]
        public string Username { get; set; }
    }

    internal class PasswordModel
    {
        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("algorithm")]
        public string Algorithm { get; set; }
    }

    public class LoginResponse
    {
        public DetailedError Error { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tokenExpires")]
        public DdpDate TokenExpires { get; set; }

        public bool HasError()
        {
            return Error != null;
        }
    }
}