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
    }
}
