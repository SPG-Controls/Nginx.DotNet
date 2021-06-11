using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nginx.Model.Models
{
    public class Http
    {
        [JsonProperty("include")]
        public string Include { get; set; }

        [JsonProperty("sendfile")]
        public bool? Sendfile { get; set; }

        [JsonProperty("client_max_body_size")]
        public string ClientMaxBodySize { get; set; }

        [JsonProperty("large_client_header_buffers")]
        public string LargeClientHeaderBuffers { get; set; }

        [JsonProperty("gzip")]
        public bool? Gzip { get; set; }

        [JsonProperty("gzip_min_length")]
        public long? GzipMinLength { get; set; }

        [JsonProperty("gzip_buffers")]
        public string GzipBuffers { get; set; }

        [JsonProperty("gzip_proxied")]
        public string GzipProxied { get; set; }

        [JsonProperty("gzip_types")]
        public string GzipTypes { get; set; }

        [JsonProperty("gzip_comp_level")]
        public long? GzipCompLevel { get; set; }

        [JsonProperty("gzip_vary")]
        public bool? GzipVary { get; set; }

        [JsonProperty("gzip_disable")]
        public string GzipDisable { get; set; }

        [JsonProperty("proxy_send_timeout")]
        public string ProxySendTimeout { get; set; }

        [JsonProperty("proxy_read_timeout")]
        public string ProxyReadTimeout { get; set; }

        [JsonProperty("fastcgi_send_timeout")]
        public string FastCgiSendTimeOut { get; set; }

        [JsonProperty("fastcgi_read_timeout")]
        public string FastCgiReadTimeOut { get; set; }

        [JsonProperty("upstream")]
        public Dictionary<string, UpstreamServer> UpstreamServers { get; set; }

        [JsonProperty("server")]
        public IEnumerable<Server> Servers { get; set; }
    }
}
