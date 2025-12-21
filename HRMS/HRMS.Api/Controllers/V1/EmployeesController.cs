using Asp.Versioning;
using Azure;
using HRMS.Application.Services.Employee;
using HRMS.SharedKernel.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;
        public EmployeesController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPaginatedEmployeesShort([FromBody] AdvanceTableRequestDto request)
        {
            var result = await _employeeServices.GetPaginatedEmployeesShortAsync(request);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEmployeeById([FromBody] IdRequestDto request)
        {
            var result = await _employeeServices.GetEmployeeByIdAsync(request.Id);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddEmployee([FromBody] InsertEmployeeRequestDto request)
        {
            var result = await _employeeServices.AddEmployeeAsync(request);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEmployeeImages([FromBody] ListIdRequestDto request)
        {
            var result = await _employeeServices.GetEmployeeImagesAsync(request);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("[action]/{id:guid}")]
        public async Task<IActionResult> GetEmployeeImage(Guid id, CancellationToken ct)
        {
            var response = await _employeeServices.GetEmployeeImageAsync(id);
            if (response is null || response.FileContent is null || response.FileContent.Length == 0)
                return NotFound();

            Response.Headers["Cache-Control"] = "private, max-age=31536000";

            return File(
                response.FileContent,
                response.FileContentType,
                enableRangeProcessing: true
            );
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEmployeeSearchListByNameOrEmail([FromBody] EmployeeSearchRequestDto request)
        {
            var result = await _employeeServices.GetEmployeeSearchListByNameOrEmailAsync(request.SearchText);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetEmployeeDetails([FromBody] IdRequestDto request)
        {
            var result = await _employeeServices.GetEmployeeDetailsAsync(request.Id);
            return Ok(result);
        }
    }
}