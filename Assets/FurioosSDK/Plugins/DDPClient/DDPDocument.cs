using Newtonsoft.Json;

namespace DdpClient
{
    public class DdpDocument
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
    }
}