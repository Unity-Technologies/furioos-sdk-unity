using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubAddedBeforeModel<T> : BaseMessageModel
    {
        public SubAddedBeforeModel()
        {
            Msg = "addedBefore";
        }
         
        [JsonProperty("collection")]
        public string Collection { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fields")]
        public T Object { get; set; }

        [JsonProperty("before")]
        public string Before { get; set; }
    }
}