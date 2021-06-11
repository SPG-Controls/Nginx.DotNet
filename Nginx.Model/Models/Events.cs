using Newtonsoft.Json;

namespace Nginx.Model.Models
{
    public class Events
    {
        [JsonProperty("worker_connections")]
        public long? WorkerConnections { get; set; }
    }
}
