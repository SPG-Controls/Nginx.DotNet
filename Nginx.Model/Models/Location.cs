using Newtonsoft.Json;
using Nginx.Model.Converters;
using System;
using System.Collections.Generic;

namespace Nginx.Model.Models
{
    public class Location
    {
        [JsonProperty("set")]
        public string Set { get; set; }

        [JsonProperty("auth_basic")]
        public string AuthBasic { get; set; }

        [JsonProperty("auth_basic_user_file")]
        public string AuthBasicUserFile { get; set; }

        [JsonProperty("proxy_pass")]
        public Uri ProxyPass { get; set; }

        [JsonProperty("proxy_redirect")]
        public bool? ProxyRedirect { get; set; }

        [JsonProperty("proxy_buffering")]
        public bool? ProxyBuffering { get; set; }

        [JsonProperty("proxy_request_buffering")]
        public bool? ProxyRequestBuffering { get; set; }

        [JsonProperty("proxy_http_version")]
        public double? ProxyHttpVersion { get; set; }

        [JsonProperty("proxy_set_header")]
        [JsonConverter(typeof(ProxyHeadersConverter))]
        public IEnumerable<string> ProxySetHeaders { get; set; }

        [JsonProperty("proxy_cache_bypass")]
        public string ProxyCacheBypass { get; set; }

        [JsonProperty("client_max_body_size")]
        public string ClientMaxBodySize { get; set; }

        [JsonProperty("try_files")]
        public string TryFiles { get; set; }

        [JsonProperty("root")]
        public string Root { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }
    }
}
