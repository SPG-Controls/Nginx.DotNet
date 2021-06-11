using Newtonsoft.Json;
using Nginx.Model.Models;
using Nginx.Model.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nginx.Model.Converters
{
    public class LocationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Location);

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            ProxyHeaderConverter proxyHeaderConverter = new ProxyHeaderConverter();

            Location location = new Location();

            List<string> proxyHeaders = new List<string>();

            // Reader past key of object
            reader.Read();

            while (reader.Read())
            {
                // Read until the end of the object is reached
                if (reader.TokenType == JsonToken.EndObject) break;

                else
                {
                    string propertyName = (string)reader.Value;

                    // Read proxy header
                    if (propertyName == "proxy_set_header")
                    {
                        proxyHeaders.Add((string)proxyHeaderConverter.ReadJson(reader, null, null, serializer));
                    }

                    else
                    {
                        GenericSerializer.ReadJson(reader, propertyName, location, typeof(Location), serializer);
                    }
                }
            }

            // Dont bother adding an empty list. Causes problems for serialising
            if (proxyHeaders.Count() > 0)
                location.ProxySetHeaders = proxyHeaders;

            return location;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
