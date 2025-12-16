using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Registrations;

using System.Threading;
using Yarp.ReverseProxy.Configuration;

public class ApiProxyConfigProvider : IProxyConfigProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private volatile IProxyConfig _config;

    public ApiProxyConfigProvider(IServiceScopeFactory scopeFactory, TimeSpan? refreshInterval = null)
    {
        _scopeFactory = scopeFactory;

        // Initialize empty config
        _config = new InMemoryConfig(Array.Empty<RouteConfig>(), Array.Empty<ClusterConfig>());

        // Fire async fetch immediately
        LoadConfigAsync().GetAwaiter().GetResult();

        // Timer for periodic refresh
        //_timer = new Timer(async _ => await LoadConfigAsync(), null, _refreshInterval, _refreshInterval);
    }

    public IProxyConfig GetConfig() => _config;

    private async Task LoadConfigAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var apiRequest = scope.ServiceProvider.GetRequiredService<ApiRequest>();
            var proxyDto = await apiRequest.PostAsync<ProxyConfigResponseDto>("/Configurations/GetProxyConfig");

            var routes = proxyDto?.Result?.Routes.Select(BuildRoute).ToList() ?? [];

            var clusters = proxyDto?.Result?.Clusters.Select(BuildCluster).ToList() ?? [];

            _config = new InMemoryConfig(routes, clusters);

            await Task.CompletedTask;


        }
        catch (Exception ex)
        {
            // Log error if needed; keep old config if API fails
            Console.WriteLine($"Failed to load proxy config: {ex.Message}");
            throw;
        }
    }
    private RouteConfig BuildRoute(RouteDto route)
    {
        return new RouteConfig
        {
            RouteId = route.RouteId,
            ClusterId = route.ClusterId,
            Match = new RouteMatch
            {
                Path = route.Path
            }
        };
    }
    private ClusterConfig BuildCluster(ClusterDto cluster)
    {
        Dictionary<string, DestinationConfig> destinations = cluster.Destinations.ToDictionary(
                    kv => kv.Key,
                    kv => new DestinationConfig { Address = kv.Value }
                );
        return new ClusterConfig
        {
            ClusterId = cluster.ClusterId,
            Destinations = destinations
        };
    }
}
