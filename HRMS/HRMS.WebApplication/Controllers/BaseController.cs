using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using HRMS.WebApplication.Extensions;
using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HRMS.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        private readonly BreadCrumbModel defaultCrumb = new() { Name = "Home", Action = "Index", Controller = "Home" };
        protected IActionResult FileResponse(FileResponseDto fileResponse)
        {
            return File(fileResponse.FileContent!, fileResponse.FileContentType, fileResponse.FileNameWithExtension);
        }
        protected IActionResult JsonResponse<T>(ApiResponseModel<T> response, string? redirectTo = "")
        {
            if(response.Success)
            {
                return Json(new { success = true, data = response.Result, message = response.Message, redirectTo });
            }

            return Json(new { success = false, data = default(T), message = response.Message });
        }

        protected IActionResult JsonBadResponse(string message = "")
        {
            return Json(new { success = false, message });
        }

        protected void InitBreadcrumbs(BreadcrumbManager breadcrumbManager, string name, bool isRootModule = false, object? routeValues = null)
        {
            if (Request.IsAjaxOrApiRequest()) return;

            var route = RouteData.Values;
            var controllerName = route["controller"]?.ToString() ?? "";
            var actionName = route["action"]?.ToString() ?? "";

            var newCrumb = new BreadCrumbModel
            {
                Name = name,
                Url = Url.Action(actionName, controllerName, routeValues) ?? string.Empty,
                IsActive = true,
                Action = actionName,
                Controller = controllerName
            };

            defaultCrumb.Url = Url.Action(defaultCrumb.Action, defaultCrumb.Controller) ?? "/";

            ViewBag.Breadcrumbs = breadcrumbManager.UpdateTrail(newCrumb, defaultCrumb, isRootModule);
        }
        protected async Task<SelectList> GetDropdownListAsync(string action, object? data = null, bool isAuthRequired = false)
        {
            var api = HttpContext.RequestServices.GetService(typeof(ApiRequest)) as ApiRequest;
            var response = await api!.PostAsync(action, data, isAuthRequired);
            var departments = response.Success
                ? response.Result as List<BaseRefDto>
                : new List<BaseRefDto>();

            return new SelectList(departments, "Id", "Name");
        }
    }
}
