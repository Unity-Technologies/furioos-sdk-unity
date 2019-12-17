using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubMovedBeforeModel : BaseMessageModel
    {
        public SubMovedBeforeModel()
        {
            Msg = "movedBefore";
        }

        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("before")]
        public string Before { get; set; }
    }
}