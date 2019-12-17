using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class NoSubModel : BaseMessageModel
    {
        public NoSubModel()
        {
            Msg = "nosub";
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("error")]
        public DetailedError Error { get; set; }
    }
}