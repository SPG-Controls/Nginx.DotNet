using Newtonsoft.Json;
using Nginx.Model.Models;
using Nginx.Model.Serializers;
using System;

namespace Nginx.Model.Converters
{
    public class UpstreamServerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(UpstreamServer);

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            UpstreamServer upstreamServer = new UpstreamServer();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propertyName = (string)reader.Value;

                    GenericSerializer.ReadJson(reader, propertyName, upstreamServer, typeof(UpstreamServer), serializer);
                }

                else if (reader.TokenType == JsonToken.EndObject)
                {
                    break;
                }
            }

            return upstreamServer;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
