using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ReferencesController : ControllerBase
    {
        public ReferencesController()
        {
            
        }
        [HttpPost("[action]")]
        public IActionResult GetAllActiveDepartments()
        {
            return Ok("References Controller is working.");
        }
    }
}
