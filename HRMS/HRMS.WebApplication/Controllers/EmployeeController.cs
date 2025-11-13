using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly ApiRequest _api;
        private readonly BreadcrumbManager _breadcrumbManager;
        public EmployeeController(ApiRequest api , BreadcrumbManager breadcrumbManager)
        {
            _api = api;
            _breadcrumbManager = breadcrumbManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? page)
        {
            InitBreadcrumbs(_breadcrumbManager, "Employess", true);

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

            InitBreadcrumbs(_breadcrumbManager, "Employee Details", routeValues: new { employeeId });
            // Logic to get employee details by id
            return View("EmployeeDetails");
        }
        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            ViewBag.ModuleTitle = "Add Employee";

            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var response = await _api.PostAsync("/Employees/GetEmployeeById", new { Id = id });

            if(!response.Success)
            {
                return View("EmployeeNotFound");
            }

            ViewBag.ModuleTitle = "Edit Employee";

            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);

            return View(response.Result);
        }
        [HttpGet]
        public IActionResult EmployeeNotFound()
        {
            return View();
        }
    }
}
