using Newtonsoft.Json;

namespace DdpClient.Models.Client
{
    public class ConnectModel : BaseMessageModel
    {
        public ConnectModel()
        {
            Msg = "connect";
        }

        [JsonProperty("session", NullValueHandling = NullValueHandling.Ignore)]
        public string Session { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("support")]
        public string[] SupportedProtocols { get; set; }
    }
}