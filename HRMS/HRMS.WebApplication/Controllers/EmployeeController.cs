using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    [Authorize]
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
                Pagination = new { PageNumber = page ?? 1, PageSize = 24 }
            },true);
            await LoadEmployeeDropdownsAsync();
            return View("Index", result.Result);
        }
        [HttpPost]
        public async Task<IActionResult> GetEmployees(AdvanceTableRequestDto request)
        {
            request ??= new();
            var result = await _api.PostAsync<PaginationResponseDto<EmployeeShortResponseDto>>("/Employees/GetPaginatedEmployeesShort", request, true);
            return PartialView("EmployeeCards", result.Result);
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

            await LoadEmployeeDropdownsAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveEmployee(InsertEmployeeRequestDto request)
        {
            var response = await _api.PostAsync("/Employees/AddEmployee", request, true);
            return JsonResponse(response, Url.Action(nameof(GetEmployees)));
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
        private async Task LoadEmployeeDropdownsAsync()
        {
            ViewBag.DepartmentList = await _api.DropdownListAsync("/References/GetAllActiveDepartments", null, true);
            ViewBag.DesignationList = await _api.DropdownListAsync("/References/GetAllActiveDesignations", null, true);
            ViewBag.GenderList = await _api.DropdownListAsync("/References/GetAllActiveGenders", null, true);
            ViewBag.MaritalStatusList = await _api.DropdownListAsync("/References/GetAllActiveMaritalStatus", null, true);
        }
        [HttpGet]
        public IActionResult EmployeeNotFound()
        {
            return View();
        }
    }
}
