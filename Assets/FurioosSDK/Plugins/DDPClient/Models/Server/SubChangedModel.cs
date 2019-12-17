using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubChangedModel<T> : BaseMessageModel
    {
        public SubChangedModel()
        {
            Msg = "changed";
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("fields")]
        public T Object { get; set; }

        [JsonProperty("cleared")]
        public string[] Cleared { get; set; }
    }
}