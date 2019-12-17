using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class ErrorModel : BaseMessageModel
    {
        public ErrorModel()
        {
            Msg = "error";
        }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("offendingMessage")]
        public string OffendingMessage { get; set; }
    }
}