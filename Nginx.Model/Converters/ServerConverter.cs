using Newtonsoft.Json;
using Nginx.Model.Models;
using Nginx.Model.Serializers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nginx.Model.Converters
{
    public class ServerConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Server);

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            LocationConverter locationConverter = new LocationConverter();

            Server server = new Server();

            Dictionary<string, Location> locations = new Dictionary<string, Location>();

            // Reader past key of object
            reader.Read();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                else
                {
                    string propertyName = (string)reader.Value;

                    // TODO get "location from JsonPropertyAttribute"
                    if (Regex.IsMatch(propertyName, "^location [^ ]+$"))
                    {
                        Location location = (Location)locationConverter.ReadJson(reader, null, null, serializer);

                        string endpoint = Regex.Replace(propertyName, "^location ", string.Empty);

                        Console.WriteLine("location endpoint: " + endpoint);

                        locations.Add(endpoint, location);
                    }

                    else
                    {
                        GenericSerializer.ReadJson(reader, propertyName, server, typeof(Server), serializer);
                    }
                }
            }

            server.Locations = locations;

            return server; 
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
