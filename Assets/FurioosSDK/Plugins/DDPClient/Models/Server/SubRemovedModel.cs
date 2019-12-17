using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubRemovedModel : BaseMessageModel
    {
        public SubRemovedModel()
        {
            Msg = "removed";
        }

        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}