using Newtonsoft.Json;

namespace DdpClient.EJson
{
    [JsonConverter(typeof(DdpJsonConverter))]
    public class DdpBinary
    {
        public byte[] Data { get; set; }
    }
}