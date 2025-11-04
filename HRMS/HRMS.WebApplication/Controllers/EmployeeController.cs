using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace HRMS.WebApplication.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly ApiRequest _api;
        public EmployeeController(ApiRequest api)
        {
            _api = api;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? page)
        {
            var result = await _api.PostAsync<PaginationResponseDto<EmployeeShortResponseDto>>("/Employees/GetPaginatedEmployeesShort", new
            {                
                Pagination = new { PageNumber = page ?? 1, PageSize = 20 }
            });

            List<EmployeeShortResponseDto> employees =
            [
                new EmployeeShortResponseDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Doe",
                    Bio = "Senior Developer with 5 years of experience."
                },
                new EmployeeShortResponseDto
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Smith",
                    Bio = "Project Manager specializing in agile methodologies."
                }
            ];
            var pagination = new PaginationResponseDto<EmployeeShortResponseDto>
            {
                Items = employees,
            };
            
            return View("Index", pagination);
        }
        [HttpPost]
        public async Task<IActionResult> GetEmployees([FromBody] AdvanceTableRequestDto request)
        {
            var result = await _api.PostAsync("/Employees/GetPaginatedEmployeesShort", request);
            return JsonResponse(result);
        }
        public IActionResult GetEmployeeDetails(Guid employeeId)
        {
            ViewBag.EmpId = employeeId;
            // Logic to get employee details by id
            return View("EmployeeDetails");
        }
        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            ViewBag.ModuleTitle = "Add Employee";
            return View();
        }
    }
}
