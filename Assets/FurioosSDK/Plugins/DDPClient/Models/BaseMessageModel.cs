using Newtonsoft.Json;

namespace DdpClient.Models
{
    public class BaseMessageModel
    {
        [JsonProperty("msg")]
        internal string Msg { get; set; }
    }
}