using Asp.Versioning;
using HRMS.Application.Services.Employee;
using HRMS.SharedKernel.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeServices _employeeServices;
        public EmployeesController(IEmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPaginatedEmployeesShort([FromBody] AdvanceTableRequestDto request)
        {
            var result = await _employeeServices.GetPaginatedEmployeesShortAsync(request);
            return Ok(result);
        }
    }
}
