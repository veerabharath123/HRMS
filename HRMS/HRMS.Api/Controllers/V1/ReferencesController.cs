using Asp.Versioning;
using HRMS.Application.Services;
using HRMS.SharedKernel.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class ReferencesController : ControllerBase
    {
        private readonly IReferenceServices _referenceServices;
        public ReferencesController(IReferenceServices referenceServices)
        {
            _referenceServices = referenceServices;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveDepartments()
        {
            var response = await _referenceServices.GetAllActiveDepartmentsAsync();
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveDesignations()
        {
            var response = await _referenceServices.GetAllActiveDesignationsAsync();
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveGenders()
        {
            var response = await _referenceServices.GetAllActiveGendersAsync();
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveMaritalStatus()
        {
            var response = await _referenceServices.GetAllActiveMaritalStatusAsync();
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveModules()
        {
            var response = await _referenceServices.GetAllActiveModulesAsync();
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllActiveModulesByParentId([FromBody] IdRequestDto request)
        {
            var response = await _referenceServices.GetAllActiveModulesByParentIdAsync(request.Id);
            return Ok(response);
        }

    }
}
