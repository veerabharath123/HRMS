using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Class.BreadCrumbs;
using HRMS.WebApplication.Extensions;
using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;

namespace HRMS.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        private readonly BreadCrumbModel defaultCrumb = new() { Name = "Home", Action = "Index", Controller = "Home" };
        protected IActionResult FileResponse(FileResponseDto fileResponse)
        {
            return File(fileResponse.FileContent!, fileResponse.FileContentType, fileResponse.FileNameWithExtension);
        }
        protected IActionResult JsonResponse<T>(ApiResponseModel<T>? response = null, string? redirectTo = "")
        {
            if(response?.Success == true)
            {
                return Json(new { success = true, data = response.Result, message = response.Message, redirectTo });
            }

            return Json(new { success = false, data = default(T), message = response?.Message });
        }

        protected IActionResult JsonBadResponse(string message = "")
        {
            return Json(new { success = false, message });
        }

        protected IActionResult ReturnView<TResult>(string viewName = "", ApiResponseModel<TResult>? result = null)
        {
            if (result is not null && result.Logout)
            {
                TempData["message"] = "Session expired, Login again.";
                return RedirectToAction("Login", "Account");
            }

            if (result is null || !result.Success || !result.HasResult)
            {
                return View("Error");
            }

            return View(viewName, result.Result);
        }
        protected IActionResult ReturnPartial<TResult>(string viewName = "", ApiResponseModel<TResult>? result = null)
        {
            if (result?.Logout == true)
            {
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    new
                    {
                        logout = true,
                        message = "Your session has expired. Please log in again."
                    }
                );
            }

            if (result is null || !result.Success || !result.HasResult)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message = "You do not have permission to access this resource."
                    }
                );
            }


            return PartialView(viewName, result.Result);
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
                Controller = controllerName,
                IsRoot = isRootModule
            };

            defaultCrumb.Url = Url.Action(defaultCrumb.Action, defaultCrumb.Controller) ?? "/";

            ViewBag.Breadcrumbs = breadcrumbManager.UpdateTrail(newCrumb, defaultCrumb);
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
