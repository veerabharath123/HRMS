using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
