using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace HRMS.WebApplication.Registrations
{
    public class InMemoryConfig : IProxyConfig
    {
        public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
        }

        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken => new Microsoft.Extensions.Primitives.CancellationChangeToken(new System.Threading.CancellationToken());
    }
}