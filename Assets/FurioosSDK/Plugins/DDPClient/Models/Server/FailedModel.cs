using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class FailedModel : BaseMessageModel
    {
        public FailedModel()
        {
            Msg = "failed";
        }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}