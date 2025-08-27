using HRMS.WebApplication.Class;
using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMS.WebApplication.Registrations
{
    public class ApiProxyConfigProvider : IProxyConfigProvider
    {
        private volatile IProxyConfig _config;
        private readonly ApiRequest _apiRequest;

        public ApiProxyConfigProvider(ApiRequest apiRequest)
        {
            _apiRequest = apiRequest;
            // Initial fetch (could be async if you want to support reloads)
            _config = new InMemoryConfig(GetRoutesFromApi().Result, GetClustersFromApi().Result);
        }

        public IProxyConfig GetConfig() => _config;

        // Example: Fetch routes from API
        private async Task<IReadOnlyList<RouteConfig>> GetRoutesFromApi()
        {
            var response = await _apiRequest.PostAsync<List<RouteConfig>>("https://your-api-url/api/proxy/routes", false);
            return response.Result ?? [];
        }

        // Example: Fetch clusters from API
        private async Task<IReadOnlyList<ClusterConfig>> GetClustersFromApi()
        {
            var response = await _apiRequest.PostAsync<List<ClusterConfig>>("https://your-api-url/api/proxy/clusters", false);
            return response.Result ?? [];
        }
    }
}
