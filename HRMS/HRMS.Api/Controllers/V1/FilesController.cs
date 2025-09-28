using HRMS.Application.Services.File;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileServices _fileServices;
        public FilesController(IFileServices fileServices)
        {
            _fileServices = fileServices;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetLanguageJson()
        {
            
            return Ok(ApiResponseDto<Dictionary<string, string>>.SuccessStatus([],"Test Json"));
        }
        [HttpPost]
        public async Task<IActionResult> UploadFileAsync([FromBody] FileRequestDto request)
        {
            var response = ApiResponseDto<bool>.SuccessStatus(true);
            return Ok(response);
        }
    }
}
