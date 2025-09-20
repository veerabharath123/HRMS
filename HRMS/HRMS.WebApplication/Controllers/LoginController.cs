using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.WebApplication.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ApiRequest _api;
        public LoginController(ApiRequest api)
        {
            _api = api;
        }
        public IActionResult AccessDenied()
        {
            return View();
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
                return View(nameof(SignUp),request);
            }

            var res = await _api.PostAsync<LoginResponseDto>("/User/SignUpUser", request);

            if(res.Success && res.Result is not null)
            {
                LoadAuthSession(res.Result);
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
                new("RequestToken",auth.Token)
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
    }
}
