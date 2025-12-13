using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HRMS.WebApplication.Controllers
{
    public class MaintenanceController : BaseController
    {
        private readonly ApiRequest _api;
        private readonly BreadcrumbManager _breadcrumbManager;
        private const string ModuleName = "Maintenance";
        public MaintenanceController(ApiRequest api, BreadcrumbManager breadcrumbManager)
        {
            _api = api;
            _breadcrumbManager = breadcrumbManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int module)
        {
            var response = await _api.PostAsync<List<ModulesResponseDto>>("/References/GetAllActiveModulesByParentId", new { Id = module }, true);
            ViewBag.ModuleTitle = ModuleName;
            InitBreadcrumbs(_breadcrumbManager, ModuleName, true, routeValues: new { module });
            return ReturnView("Index", response);
        }
        [HttpGet]
        public IActionResult UsersMaintenance()
        {
            InitBreadcrumbs(_breadcrumbManager, "Users Maintenance");
            return View("UsersMaintenance");
        }
        [HttpPost]
        public async Task<IActionResult> GetUsersMaintenance([FromBody] AdvanceTableRequestDto request)
        {
            var response = await _api.PostAsync<PaginationResponseDto<UserListResponseDto>>("/User/GetPaginatedUsers", request, true);
            return JsonResponse(response);
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.ModuleTitle = "Add User";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            return View("UserForm");
        }
    }
}
