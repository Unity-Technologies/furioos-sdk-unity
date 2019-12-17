using Newtonsoft.Json;

namespace DdpClient.Models
{
    public class DetailedError
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
    }
}