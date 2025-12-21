using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        public async Task<IActionResult> GetEmployees()
        {
            ViewBag.ModuleTitle = "Employees";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle, true);
            await LoadEmployeeDropdownsAsync();
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> GetEmployees([FromBody] AdvanceTableRequestDto request)
        {
            request ??= new();
            var result = await _api.PostAsync<PaginationResponseDto<EmployeeShortResponseDto>>("/Employees/GetPaginatedEmployeesShort", request, true);
            return ReturnPartial("EmployeeCards", result);
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
        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeRequestDto request)
        {
            var response = await _api.PostAsync("/Employees/UpdateEmployee", request, true);
            return JsonResponse(response, Url.Action(nameof(GetEmployees)));
        }
        [HttpGet]
        public async Task<IActionResult> EditEmployee(int employeeId)
        {
            var response = await _api.PostAsync<UpdateEmployeeRequestDto>("/Employees/GetEmployeeById", new { Id = employeeId }, true);

            if(!response.Success)
            {
                return View("EmployeeNotFound");
            }

            ViewBag.ModuleTitle = "Edit Employee";

            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            await LoadEmployeeDropdownsAsync();

            return ReturnView("AddEmployee",response);
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

        [HttpPost]
        public async Task<IActionResult> GetEmployeeImages([FromBody]ListIdRequestDto request)
        {
            if (request == null || request.IdList == null || request.IdList.Count == 0)
            {
                return JsonResponse<object>(null);
            }
            var response = await _api.PostAsync<List<EmpImageResponseDto>>("/Employees/GetEmployeeImages", request, true);
            return JsonResponse(response);
        }

        public async Task<IActionResult> GetEmployeeImage(Guid id, CancellationToken ct)
        {
            using var raw = await _api.GetRawFileAsync(
                $"/Employees/GetEmployeeImage/{id}",
                authRequired: true,
                cancellationToken: ct);

            if (raw == null)
                return NotFound();

            await using var ms = new MemoryStream();
            await raw.Stream.CopyToAsync(ms, ct);

            if (ms.Length == 0)
                return NotFound(); // proves empty stream

            Response.Headers["Cache-Control"] = "private, max-age=31536000";
            Response.Headers["Content-Disposition"] = "inline";

            return File(
                ms.ToArray(),
                raw.ContentType,
                enableRangeProcessing: true
            );
        }
        [HttpPost]
        public async Task<IActionResult> GetEmployeeSearchListByNameOrEmail([FromBody] EmployeeSearchRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                ApiResponseModel<List<EmployeeShortResponseDto>> defaultRes = new();
                defaultRes.Result = [];
                return JsonResponse(defaultRes);
            }

            var response = await _api.PostAsync<List<EmployeeShortResponseDto>>("/Employees/GetEmployeeSearchListByNameOrEmail", request, true);
            return JsonResponse(response);
        }
        public async Task<IActionResult> GetEmployeeDetails(int employeeId)
        {
            if (employeeId == 0)
            {
                return View("Error");
            }

            ViewBag.ModuleTitle = "Employee Details";

            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle, routeValues: new { employeeId } );

            var response = await _api.PostAsync<EmployeeDetaisResponseDto>("/Employees/GetEmployeeDetails", new { Id = employeeId }, true);
            await LoadEmployeeDropdownsAsync();

            return ReturnView("EmployeeDetails",response);
        }
    }
}
