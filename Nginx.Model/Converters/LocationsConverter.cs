using Newtonsoft.Json;
using Nginx.Model.Models;
using System;
using System.Collections.Generic;

namespace Nginx.Model.Converters
{
    public class LocationsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Dictionary<string, Location>);

        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Dictionary<string, Location> locations = (Dictionary<string, Location>)value;

            if (locations == null) return;

            foreach (var location in locations)
            {
                writer.WritePropertyName("location " + location.Key);
                serializer.Serialize(writer, location.Value);
            }
        }
    }
}
