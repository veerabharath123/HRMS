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
        #region User Maintenance
        [HttpGet]
        public IActionResult UsersMaintenance()
        {
            ViewBag.ModuleTitle = "Users Maintenance";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            return View();
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
            return View("User/AddUser");
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUser(int id)
        {
            var response = await _api.PostAsync<UserDetailsResponseDto>("/User/GetUserById", new { id }, true);

            ViewBag.ModuleTitle = "Edit User";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            return ReturnView("User/UpdateUser", response);
        }
        [HttpGet]
        public async Task<IActionResult> ViewUser(int id)
        {
            var response = await _api.PostAsync<UserDetailsResponseDto>("/User/GetUserById", new { id }, true);

            ViewBag.ModuleTitle = "View User";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            return ReturnView("UserForm", response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] UserInsertRequestDto request)
        {
            var response = await _api.PostAsync<ApiResponseDto>("/User/InsertUser", request, true); 
            return JsonResponse(response,Url.Action(nameof(UsersMaintenance)));
        }
        #endregion User Maintenance

        #region Authorization Maintenance

        [HttpGet]
        public IActionResult AuthorizationsMaintenance()
        {
            ViewBag.ModuleTitle = "Authorizations Maintenance";
            InitBreadcrumbs(_breadcrumbManager, ViewBag.ModuleTitle);
            return View();
        }

        #endregion Authorization Maintenance
    }
}
