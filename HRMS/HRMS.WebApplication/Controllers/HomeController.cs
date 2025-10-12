using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using HRMS.WebApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;

namespace HRMS.WebApplication.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiRequest _api;

        public HomeController(ILogger<HomeController> logger, ApiRequest api)
        {
            _logger = logger;
            _api = api;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult GetDatatables()
        {
            return Json(new List<object>
            {
                new  { Name = "Employees", Age = 1, Birthdate = DateTime.Now.Date, timer = "10:00", active = "Yes" },
                new  { Name = "Employees1", Age = 21, Birthdate = DateTime.Now.Date, timer = "10:30", active = "No" },
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult?> GetDocument()
        {
            var response = await _api.PostAsync<FileResponseDto>("/User/GetDocument");
            if(response.Success && response.HasResult)
            {
                return FileResponse(response.Result!);
            }

            return View("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if(file is null || file.Length == 0)
                return BadRequest("No file uploaded");
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileRequest = new FileRequestDto
            {
                FileName = file.FileName,
                FileContent = memoryStream.ToArray(),
                FileContentType = file.ContentType,
                FileExtension = Path.GetExtension(file.FileName).TrimStart('.')
            };
            var response = await _api.PostAsync<FileRequestDto, bool>("/User/UploadImage", fileRequest);
            if(response.Success && response.Result)
            {
                return RedirectToAction("Login", "Login");
            }
            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageRequestDto request)
        {
            var response = await _api.PostAsync<MessageRequestDto, bool>("/User/SendMessageByUser", request, true);

            return JsonResponse(response);
        }
        //private async Task LoadAuth(AuthResponse auth)
        //{
        //    _httpContextAccessor.HttpContext?.Session.SetString(nameof(auth.UserName), auth.UserName);
        //    var claims = new List<Claim>
        //    {
        //        new(nameof(auth.UserName), auth.UserName),
        //        new(nameof(auth.UserId), auth.UserId.ToString()),
        //        new("RequestToken",auth.Token)
        //    };

        //    if (!auth.Is2FACompleted)
        //        auth.UserPersmissions.Add("2FA");

        //    foreach (var permission in auth.UserPersmissions)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, permission));
        //    }

        //    var identity = new ClaimsIdentity(claims, GeneralConstants.CookieAuthName);
        //    var principal = new ClaimsPrincipal(identity);

        //    var authProperties = new AuthenticationProperties
        //    {
        //        IsPersistent = true,
        //        ExpiresUtc = auth.ExpiryDate,
        //    };

        //    await HttpContext.SignInAsync(GeneralConstants.CookieAuthName, principal, authProperties);
        //}

        //[HttpGet]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(GeneralConstants.CookieAuthName);
        //    _httpContextAccessor.HttpContext?.Session.Clear();

        //    return RedirectToAction(nameof(Login));
        //}
    }
}
