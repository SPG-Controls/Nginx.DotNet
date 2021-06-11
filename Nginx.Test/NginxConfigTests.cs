using FluentAssertions;
using Nginx.Model;
using Nginx.Model.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SPG.Nginx.Test
{
    public class NginxConfigTests
    {
        private static string LoadEmbeddedTextResource(string name)
        {
            using var resourceStream = typeof(NginxConfigTests).Assembly.GetManifestResourceStream(name);
            using var sr = new StreamReader(resourceStream);

            return sr.ReadToEnd();
        }

        [Fact(DisplayName = "NGINX: Parse basic configuration", Skip = "Fails in pipeline")]
        public void ParseConfigTest()
        {
            var text = LoadEmbeddedTextResource("SPG.Nginx.Test.nginx.conf");

            var results = NginxConvert.Deserialize(text);

            results.WorkerProcesses.Should().Be(2);
            results.Http.GzipMinLength.Should().Be(256);
            results.Http.GzipBuffers.Should().Be("4 32k");

            results.Http.UpstreamServers["app_servers"].Should().BeEquivalentTo(new UpstreamServer { Server = "arcoservice:5000" } );

            results.Http.Servers.Count().Should().Be(4, "Four server blocks expected");

            // Find port 80 server block
            var httpServer = results.Http.Servers.First(s => s.Listen == "80");

            httpServer.Locations["/api"].Set.Should().Be("$upstream http://app_servers");

            httpServer.Locations["/api"].ProxySetHeaders.Should().BeEquivalentTo(new List<string>
            {
                "Host $host",
                "X-Real-IP $remote_addr",
                "X-Forwarded-For $proxy_add_x_forwarded_for",
                "X-Forwarded-Host $server_name"
            });

            string serializedData = NginxConvert.Serialize(results);

            serializedData.Length.Should().BeGreaterThan(100);
        }

        [Fact(DisplayName = "NGINX: Parse alpha configuration", Skip = "Fails in pipeline")]
        public void ParseAlphaConfigTest()
        {
            var text = LoadEmbeddedTextResource("SPG.Nginx.Test.alpha.conf");

            var results = NginxConvert.Deserialize(text);

            results.WorkerProcesses.Should().Be(2);
            results.Http.GzipMinLength.Should().Be(256);
            results.Http.GzipBuffers.Should().Be("4 32k");

            results.Http.UpstreamServers["app_servers"].Should().BeEquivalentTo(new UpstreamServer { Server = "arcoservice:5000" });

            results.Http.Servers.Count().Should().Be(9, "Nine server blocks expected");

            // Find port 80 server block
            var httpServer = results.Http.Servers.First(s => s.Listen == "443 ssl");

            httpServer.Locations["/api"].ProxySetHeaders.Should().BeEquivalentTo(new List<string>
            {
                "Host $host",
                "X-Real-IP $remote_addr",
                "X-Forwarded-For $proxy_add_x_forwarded_for",
                "X-Forwarded-Host $server_name"
            });
        }

        [Fact(DisplayName = "NGINX: Save configuration")]
        public void SaveNginxConfigTest()
        {
            var config = new NginxConfiguration
            {
                Events = new Events
                {
                    WorkerConnections = 1024,
                },
                WorkerProcesses = 2,
                Http = new Http
                {
                    Include = "/etc/nginx/mime.types",
                    Sendfile = true,
                    ClientMaxBodySize = "5M",
                    LargeClientHeaderBuffers = "4 32k",

                    Gzip = true,
                    GzipMinLength = 256,
                    GzipBuffers = "4 32k",

                    UpstreamServers = new Dictionary<string, UpstreamServer>()
                    {
                        {  "app_servers", new UpstreamServer() { Server = "arcoservice:5000" } }
                    },

                    Servers = new List<Server>
                    {
                        new Server
                        {
                            Listen = "443 ssl",
                            Locations = new Dictionary<string, Location>()
                            {
                                {
                                    "/",
                                    new Location
                                    {
                                        TryFiles = "$uri /index.html",
                                        Root = "/usr/share/nginx/html",
                                        Expires = "1d"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            string data = NginxConvert.Serialize(config);

            data.Should().Contain("location / ");
        }
    }
}
