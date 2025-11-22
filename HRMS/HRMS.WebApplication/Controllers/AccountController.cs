using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.WebApplication.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ApiRequest _api;
        public AccountController(ApiRequest api)
        {
            _api = api;
        }
        public IActionResult AccessDenied()
        {
            return View("/Errors/AccessDenied");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(SignUp), request);
            }
            var res = _api.PostAsync<LoginResponseDto>("/User/Login", request).Result;

            if (res.Success && res.Result is not null)
            {
                await LoadAuthSession(res.Result);
                HttpContext.Session.SetString("KeepAlive", DateTime.Now.ToString());
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = res.Message;

            return RedirectToAction("Error");
        }
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserInsertRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(nameof(SignUp), request);
            }

            var res = await _api.PostAsync<LoginResponseDto>("/User/SignUpUser", request);

            if (res.Success && res.Result is not null)
            {
                await LoadAuthSession(res.Result);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = res.Message;

            return RedirectToAction("Error");
        }
        private async Task LoadAuthSession(LoginResponseDto auth)
        {
            HttpContext.Session.SetString("AccessToken", auth.Token);
            HttpContext?.Session.SetString(nameof(auth.UserName), auth.UserName);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, auth.UserName),
                new(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
                new("AccessToken",auth.Token)
            };

            var identity = new ClaimsIdentity(claims, "AuthCookie");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = auth.TokenExpiry,
            };

            await HttpContext!.SignInAsync("AuthCookie", principal, authProperties);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie");
            HttpContext?.Session.Clear();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult KeepAlive()
        {
            HttpContext.Session.SetString("KeepAlive", DateTime.Now.ToString());
            // Optional: store session timeout in minutes
            HttpContext.Session.SetInt32("SessionTimeoutMinutes", 1);
            return Ok();
        }
        public IActionResult SessionStatus()
        {
            // Check if session exists
            if (HttpContext.Session.IsAvailable)
            {
                // Calculate remaining time
                var sessionTimeout = HttpContext.Session.GetInt32("SessionTimeoutMinutes") ?? 30;
                var lastActivity = HttpContext.Session.GetString("KeepAlive");

                if (DateTime.TryParse(lastActivity, out var lastActivityTime))
                {
                    var elapsed = DateTime.Now - lastActivityTime;
                    var remainingSeconds = (int)((sessionTimeout * 60) - elapsed.TotalSeconds);
                    remainingSeconds = Math.Max(remainingSeconds, 0);

                    return Json(new { expired = remainingSeconds <= 0, remainingSeconds });
                }
            }

            return Json(new { expired = true, remainingSeconds = 0 });
        }
        public IActionResult UnauthorizedPage()
        {
            var user = HttpContext.User;

            // Case 1: Not logged in or expired
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return View("/Errors/Unauthorized");
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
