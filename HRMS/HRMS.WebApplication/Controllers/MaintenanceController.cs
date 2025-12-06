using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetUsersMaintenance()
        {
            InitBreadcrumbs(_breadcrumbManager, "Users Maintenance", true);
            return View("UsersMaintenance");
        }
    }
}
