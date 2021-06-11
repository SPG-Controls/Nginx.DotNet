using Newtonsoft.Json;
using Nginx.Model.Models;
using Nginx.Model.Serializers;
using System;
using System.Collections.Generic;

namespace Nginx.Model.Converters
{
    public class ServersConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IEnumerable<Server>);

        public override bool CanRead => false;

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var locationConveter = new LocationsConverter();

            // Get list of servers
            var servers = (value as IEnumerable<Server>);

            // Skip if servers have not been set
            if (servers == null) return;

            // Serialise all servers
            foreach (var server in servers)
            {
                // Write start of server object
                writer.WritePropertyName("server");

                // Write object for the server
                writer.WriteStartObject();

                // Serialise all properties in server object
                foreach (var prop in typeof(Server).GetProperties())
                {
                    // Serialise dictionary of locations
                    if (prop.PropertyType == typeof(Dictionary<string, Location>))
                    {
                        locationConveter.WriteJson(writer, prop.GetValue(server, null), serializer);
                    }

                    // Serialise all other properties
                    else
                    {
                        GenericSerializer.WriteJson(writer, server, prop, serializer);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
