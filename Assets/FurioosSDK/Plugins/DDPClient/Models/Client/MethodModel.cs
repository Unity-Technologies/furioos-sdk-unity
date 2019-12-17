using Newtonsoft.Json;

namespace DdpClient.Models.Client
{
    public class MethodModel : BaseMessageModel
    {
        public MethodModel()
        {
            Msg = "method";
        }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object[] Params { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}