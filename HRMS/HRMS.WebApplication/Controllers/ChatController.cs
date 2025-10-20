using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HRMS.WebApplication.Controllers
{
    public class ChatController : BaseController
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
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequestDto request)
        {
            if(ModelState.IsValid)
            {
                var response = await _api.PostAsync<bool>("/User/SendMessageByUser", request, true);

                return JsonResponse(response);
            }

            return JsonBadResponse("Invalid data");
        }
    }
}
