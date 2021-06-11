using Nginx.Model.Models;
using System.Collections.Generic;

namespace Nginx.Model
{
    public static class NginxBuilder
    {
        public static Dictionary<string, UpstreamServer> BuildUpstreamServers()
        {
            return new Dictionary<string, UpstreamServer>();
        }

        public static Server BuildServer(string listen, string serverName)
        {
            return new Server
            {
                Listen = listen,
                ServerName = serverName,
                Locations = new Dictionary<string, Location>()
            };
        }

        public static Server BuildServer(int listen, string serverName) => BuildServer(listen.ToString(), serverName);

        public static Server BuildServer(int listen) => BuildServer(listen.ToString());

        public static Server BuildServer(string listen) => new Server { Listen = listen, Locations = new Dictionary<string, Location>() };
    }
}
