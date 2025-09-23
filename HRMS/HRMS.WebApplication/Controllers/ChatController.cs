using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HRMS.WebApplication.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApiRequest _api;
        public ChatController(ApiRequest api)
        {
            _api = api;
        }
        public async Task<IActionResult> Chats()
        {
            var response = await _api.PostAsync<List<ChatUserResponseDto>>("/User/GetUsers", true);
            var chats = new List<ChatUserResponseDto>();

            if (response.Success && response.Result is not null)
                chats = response.Result;


            return View("Chat",chats);
        }
    }
}
