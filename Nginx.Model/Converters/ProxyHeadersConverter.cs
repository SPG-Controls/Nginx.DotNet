using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nginx.Model.Converters
{
    public class ProxyHeadersConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable<string>);

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var headerList = (value as IEnumerable<string>).ToList();

            if (headerList != null && headerList.Any())
            {
                var first = headerList.First();
                headerList.Remove(first);

                var token = JToken.FromObject(first);
                token.WriteTo(writer);

                foreach (var item in headerList)
                {
                    writer.WritePropertyName("proxy_set_header");
                    writer.WriteValue(item.ToString());
                }
            }
        }
    }
}
