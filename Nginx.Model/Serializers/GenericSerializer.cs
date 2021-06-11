using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Nginx.Model.Serializers
{
    public static class GenericSerializer
    {
        public static void ReadJson(JsonReader reader, string propertyName, object targetObj, Type type, JsonSerializer serializer)
        {
            // Read value
            reader.Read();

            var prop = type.GetProperties()
                .FirstOrDefault(prop => prop
                    .GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                    .OfType<JsonPropertyAttribute>()
                    .FirstOrDefault()?.PropertyName == propertyName || prop.Name == propertyName);

            // A property should always exist
            if (prop == null)
            {
                // Throw error for now
                throw new JsonSerializationException($"Invalid property in JSON {propertyName}");

                // TODO deserialise until next property at the same depth
            }

            else
            {
                var obj = serializer.Deserialize(reader, prop.PropertyType);

                prop.SetValue(targetObj, obj);
            }
        }

        public static void WriteJson(JsonWriter writer, object targetObj, PropertyInfo prop, JsonSerializer serializer)
        {
            // Get the value of the property in the given http object
            var propValue = prop.GetValue(targetObj, null);

            // Skip if there is no value for this property
            if (propValue == null) return;

            // Get the name of the property from json attributes
            var propertyName = prop
                .GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                .OfType<JsonPropertyAttribute>()
                .FirstOrDefault()?.PropertyName ?? prop.Name;

            // Serialise and write the object
            writer.WritePropertyName(propertyName);
            serializer.Serialize(writer, propValue);
        }
    }
}
