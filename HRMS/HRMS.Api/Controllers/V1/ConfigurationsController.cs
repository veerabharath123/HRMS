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
        [HttpPost("[action]")]
        public IActionResult GetProxyConfig()
        {
            var proxyConfig = new ProxyConfigResponseDto
            {
                Routes =
                [
                    new() {
                        RouteId = "notificationsRoute",
                        ClusterId = "notificationHubCluster",
                        Path = "/hubs/notifications/{**catch-all}"
                    }
                ],
                Clusters =
                [
                    new() {
                        ClusterId = "notificationHubCluster",
                        Destinations = new Dictionary<string, string>
                        {
                            { "dest1", "https://localhost:7163/" }
                        }
                    }
                ]
            };

            return Ok(ApiResponseDto<ProxyConfigResponseDto>.SuccessStatus(proxyConfig));
        }

    }
}
