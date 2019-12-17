using Newtonsoft.Json;

namespace DdpClient.Models
{
    public class PongModel : BaseMessageModel
    {
        public PongModel()
        {
            Msg = "pong";
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}