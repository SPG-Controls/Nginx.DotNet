using Newtonsoft.Json;
using Nginx.Model.Models;
using Nginx.Model.Serializers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nginx.Model.Converters
{
    public class HttpConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Http);

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Initialise converters for sub classes
            var upstreamServerConverter = new UpstreamServerConverter();
            var serverConverter = new ServerConverter();

            Http http = new Http();

            Dictionary<string, UpstreamServer> upstreamServers = new Dictionary<string, UpstreamServer>();
            List<Server> servers = new List<Server>();

            while (reader.Read())
            {
                // Stop reading when the end of the object is reached
                if (reader.TokenType == JsonToken.EndObject) break;

                else
                {
                    string propertyName = (string)reader.Value;

                    // Add upstream server
                    // TODO get "upstream" from JsonPropertyAttribute
                    if (Regex.IsMatch(propertyName, "^upstream [^ ]+$"))
                    {
                        upstreamServers.Add(
                            Regex.Replace(propertyName, "^upstream ", string.Empty),
                            (UpstreamServer)upstreamServerConverter.ReadJson(reader, null, null, serializer)
                        );
                    }

                    // Add server
                    // TODO get "server" from JsonPropertyAttribute
                    else if (propertyName == "server")
                    {
                        servers.Add((Server)serverConverter.ReadJson(reader, null, null, serializer));
                    }

                    // Deserialise all other properties
                    else
                    {
                        GenericSerializer.ReadJson(reader, propertyName, http, typeof(Http), serializer);
                    }
                }
            }

            // Set servers if any exist. May cause serialsation error if empty

            if (upstreamServers.Count > 0)
                http.UpstreamServers = upstreamServers;

            if (servers.Count > 0)
                http.Servers = servers;

            return http;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Initialise converters for sub classes
            UpstreamServersConverter upstreamServerConverter = new UpstreamServersConverter();
            ServersConverter serverConverter = new ServersConverter();

            Http http = value as Http;

            // Object to write http properties to
            writer.WriteStartObject();

            // Write all properties
            foreach (var prop in typeof(Http).GetProperties())
            {
                // Serialise upstream server
                if (prop.PropertyType == typeof(Dictionary<string, UpstreamServer>))
                {
                    upstreamServerConverter.WriteJson(writer, prop.GetValue(http, null), serializer);
                }

                // Serialise server
                else if (prop.PropertyType == typeof(IEnumerable<Server>))
                {
                    serverConverter.WriteJson(writer, prop.GetValue(http, null), serializer);
                }

                // Serialise all other property types
                else
                {
                    GenericSerializer.WriteJson(writer, http, prop, serializer);
                }
            }

            writer.WriteEndObject();
        }
    }
}
