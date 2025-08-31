using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        public FilesController()
        {
        }

        public async Task<IActionResult> GetLanguageJson()
        {
            
            return Ok(ApiResponseDto<Dictionary<string, string>>.SuccessStatus([],"Test Json"));
        }
    }
}
