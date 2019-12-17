using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class ConnectResponse : BaseMessageModel
    {
        public ConnectResponse()
        {
            Msg = "connected";
        }

        public FailedModel Failed { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        public bool DidFail()
        {
            return Failed != null;
        }
    }
}