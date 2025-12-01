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
            var response = await _api.PostAsync<List<ChatConversationListResponseDto>>("/Chats/GetChatConversationList", true);
            var chats = new List<ChatConversationListResponseDto>();

            if (response.Success && response.Result is not null)
                chats = response.Result;


            return View("Chat",chats);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequestDto request)
        {
            if(ModelState.IsValid)
            {
                var response = await _api.PostAsync<ChatMessageResponseDto>("/Chats/SendMessage", request, true);

                return PartialView("ChatMessage",response.Result);
            }

            throw new Exception("Invalid data");
        }
        [HttpPost]
        public async Task<IActionResult> StartNewChatWith([FromBody] IdRequestDto request)
        {
            if(ModelState.IsValid)
            {
                var response = await _api.PostAsync<List<ChatConversationListResponseDto>>("/Chats/StartNewChatWith", request, true);

                return PartialView("ChatUserList",response.Result);
            }

            throw new Exception("Invalid data");
        }
        [HttpPost]
        public async Task<IActionResult> GetChatConversationDetails([FromBody] IdRequestDto request)
        {
            if(ModelState.IsValid)
            {
                var response = await _api.PostAsync<ChatConversationDetailResponseDto>("/Chats/GetChatConversationDetails", request, true);

                return PartialView("ChatScreen",response.Result);
            }

            throw new Exception("Invalid data");
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessageHtml([FromBody] ChatMessageResponseDto message)
        {
            var response = await _api.PostAsync("/Chats/MarkMessageAsDelivered", new { message.Id }, true);
            return PartialView("ChatMessage", message);
        }
    }
}
