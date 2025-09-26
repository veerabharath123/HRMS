using Asp.Versioning;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ConfigurationsController : ControllerBase
    {
        private readonly ILogger<ConfigurationsController> _logger;
        public ConfigurationsController(ILogger<ConfigurationsController> logger)
        {
            _logger = logger;
        }
        [HttpPost("[action]")]
        public IActionResult GetProxyConfig()
        {
            var builder = new UriBuilder
            {
                Scheme = Request.Scheme,
                Host = Request.Host.Host,
                Port = Request.Host.Port ?? (Request.Scheme == "https" ? 80 : 443),
                Path = Request.PathBase.Value?.TrimEnd('/') ?? string.Empty
            };

            var destination = new Dictionary<string, string> { ["dest1"] = builder.ToString() };

            string[] hubs = ["notification", "chat"];

            var routes = hubs.Select(hub => new RouteDto
            {
                RouteId = $"{hub}Route",
                ClusterId = $"{hub}HubCluster",
                Path = $"/hubs/{hub}/{{**catch-all}}"
            }).ToList();

            var clusters = hubs.Select(hub => new ClusterDto
            {
                ClusterId = $"{hub}HubCluster",
                Destinations = destination
            }).ToList();

            var proxyConfig = new ProxyConfigResponseDto { Routes = routes, Clusters = clusters };

            return Ok(ApiResponseDto<ProxyConfigResponseDto>.SuccessStatus(proxyConfig));
        }

    }
}
