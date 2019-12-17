using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DdpClient.Models
{
    public class MethodResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("error")]
        public DetailedError Error { get; set; }

        [JsonProperty("result")]
        public dynamic Result { get; set; }

        public bool HasError()
        {
            return Error != null;
        }

        public T Get<T>()
        {
            if (Result is JObject)
                return ((JObject) Result).ToObject<T>();
            if (Result is JValue)
                return ((JValue) Result).ToObject<T>();
			if (Result is JArray)
				return ((JArray)Result).ToObject<T>();
            return default(T);
        }
    }
}