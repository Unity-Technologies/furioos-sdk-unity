using System.Collections.Generic;
using Newtonsoft.Json;

namespace DdpClient.Models.Server
{
    public class SubReadyModel : BaseMessageModel
    {
        public SubReadyModel()
        {
            Msg = "ready";
        }

        [JsonProperty("subs")]
        public List<string> Subs { get; set; }
    }
}