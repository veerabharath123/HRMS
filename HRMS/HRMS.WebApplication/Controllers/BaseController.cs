using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult FileResponse(FileResponseDto fileResponse)
        {
            return File(fileResponse.FileContent!, fileResponse.FileContentType, fileResponse.FileNameWithExtension);
        }
        protected IActionResult JsonResponse<T>(Models.ApiResponseModel<T> response)
        {
            if(response.Success)
            {
                return Json(new { success = true, data = response.Result, message = response.Message });
            }

            return Json(new { success = false, data = default(T), message = response.Message });
        }

        protected IActionResult JsonBadResponse(string message = "")
        {
            return Json(new { success = false, data = string.Empty, message });
        }
    }
}
