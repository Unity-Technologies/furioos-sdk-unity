using System;
using Newtonsoft.Json;

namespace DdpClient.EJson
{
    [JsonConverter(typeof(DdpJsonConverter))]
    public class DdpDate
    {
        public DateTime DateTime { get; set; }
    }
}