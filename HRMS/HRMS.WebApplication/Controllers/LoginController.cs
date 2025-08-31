using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    public class LoginController : BaseController
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
