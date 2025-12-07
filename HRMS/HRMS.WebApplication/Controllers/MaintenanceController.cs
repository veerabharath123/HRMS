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
        public MaintenanceController(ApiRequest api, BreadcrumbManager breadcrumbManager)
        {
            _api = api;
            _breadcrumbManager = breadcrumbManager;
        }
        public IActionResult Index()
        {
            InitBreadcrumbs(_breadcrumbManager, "Maintenance", true);
            return View();
        }
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
    }
}
