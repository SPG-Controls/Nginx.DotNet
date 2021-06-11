using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nginx.Model.Models
{
    public class Server
    {
        [JsonProperty("listen")]
        public string Listen { get; set; }

        [JsonProperty("server_name")]
        public string ServerName { get; set; }

        [JsonProperty("return")]
        public string Return { get; set; }

        [JsonProperty("error_log /dev/stdout")]
        public string ErrorLogStdOut { get; set; }

        [JsonProperty("keepalive_timeout")]
        public string KeepAliveTimeout { get; set; }

        [JsonProperty("ssl_certificate")]
        public string SslCertificate { get; set; }

        [JsonProperty("ssl_certificate_key")]
        public string SslCertificateKey { get; set; }

        [JsonProperty("ssl_session_timeout")]
        public string SslSessionTimeout { get; set; }

        [JsonProperty("ssl_protocols")]
        public string SslProtocols { get; set; }

        [JsonProperty("ssl_ciphers")]
        public string SslCiphers { get; set; }

        [JsonProperty("ssl_session_cache")]
        public string SslSessionCache { get; set; }

        [JsonProperty("ssl_prefer_server_ciphers")]
        public bool? SslPreferServerCiphers { get; set; }

        [JsonProperty("ssl_buffer_size")]
        public string SslBufferSize { get; set; }

        [JsonProperty("proxy_ssl_session_reuse")]
        public bool? ProxySslSessionReuse { get; set; }

        [JsonProperty("proxy_connect_timeout")]
        public string ProxyConnectTimeout { get; set; }

        [JsonProperty("proxy_send_timeout")]
        public string ProxySendTimeout { get; set; }

        [JsonProperty("proxy_read_timeout")]
        public string ProxyReadTimeout { get; set; }

        [JsonProperty("location")]
        public Dictionary<string, Location> Locations { get; set; }
    }
}
