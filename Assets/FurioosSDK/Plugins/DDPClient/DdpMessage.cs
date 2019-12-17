using Newtonsoft.Json.Linq;

namespace DdpClient
{
    public class DdpMessage
    {
        public DdpMessage(string msg, string body)
        {
            Msg = msg;
            Body = JObject.Parse(body);
        }

        public string Msg { get; set; }
        public JObject Body { get; set; }

        public T Get<T>()
        {
            return Body.ToObject<T>();
        }
    }
}