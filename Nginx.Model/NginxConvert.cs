using Newtonsoft.Json;
using Nginx.Model.Extensions;
using Nginx.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nginx.Model
{
    public class JsonProperty
    {
        public string Name { get; set; }

        public Type PropertyType { get; set; }

        public JsonPropertyAttribute Attribute { get; set; }
    }

    public static class NginxConvert
    {
        public static string Serialize(NginxConfiguration configuration)
        {
            string result = JsonConvert.SerializeObject(configuration, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return JsonToNginxConfiguration(result);
        }

        public static NginxConfiguration Deserialize(string data)
        {
            // Convert the NGINX config into valid JSON
            data = NginxConfigurationToJson(data);

            // And then deserialize out to a concrete object
            return JsonConvert.DeserializeObject<NginxConfiguration>(data);
        }

        private static string NginxConfigurationToJson(string data)
        {
            // Strip all comments
            data = string.Join('\r', data.Split('\r').Select(line => line.StripLineComment()));

            // Remove 1 indentation and blank lines
            var lines = data
                .Split(Environment.NewLine)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line = "  " + line);

            data = string.Join(Environment.NewLine, lines);

            // Check each property
            var properties = GetYmlProperties();

            foreach (var property in properties)
            {
                // Add a colon to the property
                string alias = property.Attribute.PropertyName;

                bool treatAsString = property.PropertyType == typeof(string) || property.PropertyType == typeof(Uri) || property.PropertyType == typeof(IEnumerable<string>);

                bool isComplexProperty = !property.PropertyType.IsValueType && !treatAsString;

                // Get all occurances of the property
                int startIndex = -1;

                if (isComplexProperty)
                {
                    // Regex matches complex properties with and without values in the key
                    string complexPropertyLineRegex = $"^[ \t]*{alias}.*{{";

                    while (true)
                    {
                        // Get index of the next occurance of the complex property
                        startIndex = data.RegexIndexOf(complexPropertyLineRegex, startIndex + 1);

                        // Check if there are no more occurances of the property
                        if (startIndex < 0) break;

                        // move the start index to the position of the alias (skip blank spaces)
                        startIndex = data.IndexOf(alias, startIndex);

                        // Get the end of the current line
                        int endIndex = data.IndexOf(" {", startIndex);

                        // Get alias with possble value with it e.g. upstream app_servers {
                        string newAlias = data[startIndex..endIndex];

                        // Convert
                        data = data.Remove(startIndex, endIndex - startIndex);
                        data = data.Insert(startIndex, $"\"{newAlias}\":");
                    }
                }

                else
                {
                    // Regex matches complex properties with and without values in the key
                    string lineRegex = $"^[ \t]*{alias} .*;";

                    while (true)
                    {
                        startIndex = data.RegexIndexOf(lineRegex, startIndex + 1);

                        if (startIndex < 0) break;

                        // move the start index to the position of the alias (skip blank spaces)
                        startIndex = data.IndexOf(alias, startIndex);

                        int endIndex = data.IndexOf(';', startIndex);

                        string propertyValue = GetPropertyValue(data, alias, startIndex, endIndex);

                        if (treatAsString)
                        {
                            // Replace invalid characters
                            propertyValue = propertyValue.Replace(@"\", @"\\").Replace("\"", "\\\"");

                            // Add quotes around any string properties
                            propertyValue = $"\"{propertyValue}\"";
                        }

                        // Convert on/off to true/false (TODO: Can probably do this using a converter)
                        else if (property.PropertyType == typeof(bool?))
                        {
                            if (propertyValue == "on")
                                propertyValue = "true";
                            else if (propertyValue == "off")
                                propertyValue = "false";
                        }

                        data = data.Remove(startIndex, endIndex - startIndex + 1); // +1 to strip the semi-colon
                        data = data.Insert(startIndex, $"\"{alias}\": {propertyValue},");
                    }
                }
            }

            // Throw a comma after all close curly brackets
            data = data.Replace("}", "},");

            // Add brackets at the start & end
            data = "{" + Environment.NewLine + data + Environment.NewLine + "}";

            // Remove last comma before a close curly bracket
            // Note: Newtonsoft seems okay with it, but some JSON parsers can be picky with an extra comma
            int index = -1;

            while (true)
            {
                index = data.RegexIndexOf(",[ \r\n]*}", index + 1);

                if (index < 0) break;

                data = data.Remove(index, 1);
            }

            return data;
        }

        private static string JsonToNginxConfiguration(string data)
        {
            // Check each property
            var properties = GetYmlProperties();

            foreach (var property in properties)
            {
                if (property.Attribute == null) continue;

                // Nginx property name
                string alias = property.Attribute.PropertyName;

                // Check if the value is a string in the JSON
                bool treatAsString = property.PropertyType == typeof(string) || property.PropertyType == typeof(Uri) || property.PropertyType == typeof(IEnumerable<string>);

                // Check if property is a key with a JSON object as the value
                bool isComplexProperty = !property.PropertyType.IsValueType && !treatAsString;

                // Start looking for occurances of the property from the beginning of the JSON string
                int startIndex = -1;

                // If a property is a key with a JSON object as the value
                if (isComplexProperty)
                {
                    // Regex matches complex properties with and without values in the key
                    string complexPropertyRegex = $"\"{alias}.*\": {{";

                    while (true)
                    {
                        // Get index of the next occurance of the complex property
                        startIndex = data.RegexIndexOf(complexPropertyRegex, startIndex + 1);

                        // Check if there are no more occurances of the property
                        if (startIndex < 0) break;

                        // Get the end of the current line
                        int endIndex = data.IndexOf(Environment.NewLine, startIndex);

                        // Extract value out of key as well if one exists e.g. upstream app_servers {
                        int colonIndex = data.IndexOf(':', startIndex);
                        string newAlias = data[startIndex..colonIndex].Trim('"');

                        // Convert
                        data = data.Remove(startIndex, endIndex - startIndex);
                        data = data.Insert(startIndex, $"{newAlias} {{");
                    }
                }

                // If property is a key with a value other than another JSON object
                else
                {
                    // How the property appears in JSON
                    string aliasWithColon = $"\"{alias}\":";

                    // Process all occurances of the property
                    while (true)
                    {
                        // Get index of the next occurance of the property
                        startIndex = data.IndexOf(aliasWithColon, startIndex + 1);

                        // Check if there are no more occurances of the property
                        if (startIndex < 0) break;

                        // Get the end of line
                        int endIndex = data.IndexOf(Environment.NewLine, startIndex);

                        // Length of property + value
                        int length = endIndex - startIndex;

                        // Get the value of the property
                        string propertyValue = GetPropertyValue(data, aliasWithColon, startIndex, endIndex);

                        // Process string property
                        if (treatAsString)
                        {
                            // Remove wrapping quotes
                            propertyValue = propertyValue[1..^1];

                            // Replace invalid characters
                            propertyValue = propertyValue.Replace(@"\\", @"\").Replace("\\\"", "\"");
                        }

                        // Convert on/off to true/false (TODO: use a converter)
                        else if (property.PropertyType == typeof(bool?))
                        {
                            if (propertyValue == "true")
                                propertyValue = "on";
                            else if (propertyValue == "false")
                                propertyValue = "off";
                        }

                        // Convert
                        data = data.Remove(startIndex, length);
                        data = data.Insert(startIndex, $"{alias} {propertyValue};");
                    }
                }
            }

            // Strip a comma after all close curly brackets
            data = data.Replace("},", "}");

            // Also trim brackets at the start & end
            data = data.Trim().Trim('{', '}');

            // Remove 1 indentation and blank lines
            var nonEmptyLines = data
                .Split(Environment.NewLine)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Substring(2));

            data = string.Join(Environment.NewLine, nonEmptyLines);

            return data;
        }

        private static string GetPropertyValue(string data, string propertyName, int startIndex, int endIndex)
        {
            int valueIndex = startIndex + propertyName.Length;
            int length = endIndex - valueIndex;

            return data.Substring(valueIndex, length).Trim(' ', ',');
        }

        private static List<Type> TypeList = new List<Type>
        {
            typeof(NginxConfiguration),
            typeof(Events),
            typeof(Http),
            typeof(Server),
            typeof(Location),
            typeof(UpstreamServer)
        };

        private static IEnumerable<JsonProperty> GetYmlProperties()
        {
            return TypeList
                .SelectMany(t => t.GetProperties())
                .Select(p => new JsonProperty
                {
                    Name = p.Name,
                    Attribute = p.GetCustomAttributes(typeof(JsonPropertyAttribute), false).OfType<JsonPropertyAttribute>().FirstOrDefault(),
                    PropertyType = p.PropertyType
                });
        }
    }
}
