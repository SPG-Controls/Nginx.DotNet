using Newtonsoft.Json;

namespace Nginx.Model.Models
{
    public class UpstreamServer
    {
        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("keepalive")]
        public long? Keepalive { get; set; }
    }
}
