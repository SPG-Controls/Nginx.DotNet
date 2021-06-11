using Newtonsoft.Json;
using Nginx.Model.Converters;
using Nginx.Model.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Nginx.Model
{
    public class NginxConfiguration
    {
        [JsonProperty("worker_processes")]
        public long? WorkerProcesses { get; set; }

        [JsonProperty("events")]
        public Events Events { get; set; }

        [JsonProperty("http")]
        [JsonConverter(typeof(HttpConverter))]
        public Http Http { get; set; }

        public static NginxConfiguration Default()
        {
            return new NginxConfiguration
            {
                WorkerProcesses = 2,
                Events = new Events { WorkerConnections = 1024 },
                Http = new Http
                {
                    Include = "/etc/nginx/mime.types",
                    Sendfile = true,
                    ClientMaxBodySize = "5M",
                    LargeClientHeaderBuffers = "4 32k"
                }
            };
        }

        public static bool TryReadFromFile(string filename, out NginxConfiguration nginxConfiguration)
        {
            try
            {
                string configurationYml = File.ReadAllText(filename);
                nginxConfiguration = NginxConvert.Deserialize(configurationYml);
                return true;
            }

            catch (Exception)
            {
                nginxConfiguration = null;
                return false;
            }
        }

        public static async Task<string> WriteToFileAsync(NginxConfiguration nginxConfiguration, string filename)
        {
            string nginxYml = NginxConvert.Serialize(nginxConfiguration);

            // Create the folder if it doesn't exist
            string dir = Path.GetDirectoryName(filename);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllTextAsync(filename, nginxYml);

            return nginxYml;
        }
    }
}
