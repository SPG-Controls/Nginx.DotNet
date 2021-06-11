using System;
using System.IO;
using System.Threading.Tasks;

namespace Nginx.Model
{
    public class NginxWriter
    {
        public static async Task<string> WriteToFileAsync(NginxConfiguration configuration, string filename)
        {
            string configurationYml = NginxConvert.Serialize(configuration);

            if (string.IsNullOrEmpty(configurationYml))
                throw new Exception("Error writing nginx");

            // Create the folder if it doesn't exist
            string dir = Path.GetDirectoryName(filename);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllTextAsync(filename, configurationYml);

            return configurationYml;
        }
    }
}
