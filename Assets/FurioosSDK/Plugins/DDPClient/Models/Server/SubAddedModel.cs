using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubAddedModel<T> : BaseMessageModel
    {
        public SubAddedModel()
        {
            Msg = "added";
        }

        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fields")]
        public T Object { get; set; }
    }
}