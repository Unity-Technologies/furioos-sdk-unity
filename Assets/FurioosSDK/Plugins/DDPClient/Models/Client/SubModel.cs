using Newtonsoft.Json;

namespace DdpClient.Models.Client
{
    public class SubModel : BaseMessageModel
    {
        public SubModel()
        {
            Msg = "sub";
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("params")]
        public object[] Params { get; set; }
    }
}