using System;
using System.Collections.Generic;
using DdpClient.EJson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DdpClient
{
    public class DdpJsonConverter : JsonConverter
    {
        private readonly Dictionary<Type, Func<JsonReader, object, JsonSerializer, object>> _readTypes;

        public DdpJsonConverter()
        {
            _readTypes = new Dictionary<Type, Func<JsonReader, object, JsonSerializer, object>>
            {
                [typeof (DdpDate)] = (reader, existingValue, serializer) =>
                {
                    JObject ob = JObject.Load(reader);
                    return ob["$date"] == null
                        ? existingValue
                        : new DdpDate
                        {
                            DateTime = DdpUtil.MillisecondsToDateTime(ob["$date"].ToObject<long>())
                        };
                },
                [typeof (DdpBinary)] = (reader, existingValue, serializer) =>
                {
                    JObject ob = JObject.Load(reader);
                    return ob["$binary"] == null
                        ? existingValue
                        : new DdpBinary
                        {
                            Data = DdpUtil.GetBytesFromBase64(ob["$binary"].ToObject<string>())
                        };
                }
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DdpBinary)
            {
                DdpBinary ddpBinary = (DdpBinary)value;
                writer.WriteStartObject();
                writer.WritePropertyName("$binary");
                writer.WriteValue(DdpUtil.GetBase64FromBytes(ddpBinary.Data));
                writer.WriteEndObject();
                return;
            }
            if (value is DdpDate)
            {
                DdpDate ddpDate = (DdpDate) value;
                writer.WriteStartObject();
                writer.WritePropertyName("$date");
                writer.WriteValue(DdpUtil.DateTimeToMilliseconds(ddpDate.DateTime));
                writer.WriteEndObject();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return _readTypes.ContainsKey(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return _readTypes[objectType](reader, existingValue, serializer);
        }
    }
}