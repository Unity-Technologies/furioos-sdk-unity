using Newtonsoft.Json;

namespace DdpClient.Models
{
    public class PingModel : BaseMessageModel
    {
        public PingModel()
        {
            Msg = "ping";
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}