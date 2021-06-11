using Newtonsoft.Json;
using Nginx.Model.Models;
using System;
using System.Collections.Generic;

namespace Nginx.Model.Converters
{
    public class UpstreamServersConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Dictionary<string, UpstreamServer>);

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<string, UpstreamServer> upstreamServers = (Dictionary<string, UpstreamServer>)value;

            foreach (var upstreamServer in upstreamServers)
            {
                writer.WritePropertyName("upstream " + upstreamServer.Key);
                serializer.Serialize(writer, upstreamServer.Value);
            }
        }
    }
}
