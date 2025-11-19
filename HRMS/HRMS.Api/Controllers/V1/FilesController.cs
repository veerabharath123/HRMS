using Asp.Versioning;
using HRMS.Application.Services.File;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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
            
            return Ok(ApiResponseDto.SuccessStatus(new Dictionary<string,string>(),"Test Json"));
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadFile([FromBody] FileRequestDto request)
        {
            var response = await _fileServices.SaveFileAsync(request);
            return Ok(response);
        }
    }
}
