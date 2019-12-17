using Newtonsoft.Json;

namespace DdpClient.Models.Client
{
    public class UnsubModel : BaseMessageModel
    {
        public UnsubModel()
        {
            Msg = "unsub";
        }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}