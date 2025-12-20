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
            ViewBag.ModuleTitle = "Chats";
            var response = await _api.PostAsync<List<ChatConversationListResponseDto>>("/Chats/GetChatConversationList", true);
            var chats = new List<ChatConversationListResponseDto>();

            if (response.Success && response.Result is not null)
                chats = response.Result;


            return View("Chat",chats);
        }
        public async Task<IActionResult> GetPreviousMessages([FromBody] NextMessagesRequestDto request)
        {
            var response = await _api.PostAsync<List<ChatMessageResponseDto>>("/Chats/GetPreviousMessages", request, true);
            var chats = new List<ChatMessageResponseDto>();

            if (response.Success && response.Result is not null)
                chats = response.Result;


            return ReturnPartial("ChatMessages", response);
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
        [HttpPost]
        public async Task<IActionResult> MarkMessageAsRead([FromBody] UpdateSeenRequestDto request)
        {
            var response = await _api.PostAsync("/Chats/MarkMessageAsRead", request, true);
            return JsonResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> MarkMessageAsDelivered([FromBody] ChatMessageResponseDto message)
        {
            var response = await _api.PostAsync("/Chats/MarkMessageAsDelivered", new { message.Id }, true);
            return JsonResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> SendTypingStatus([FromBody] TypingRequestDto request)
        {
            var response = await _api.PostAsync("/Chats/SendTypingStatus", request, true);
            return JsonResponse(response);
        }
        [HttpPost]
        public async Task<IActionResult> GetAttachmentFile([FromBody] GuidIdRequestDto request)
        {
            var response = await _api.PostAsync<FileBase64ResponseDto>("/Chats/GetAttachmentFile", request, true);
            return JsonResponse(response);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadAttachmentFile(Guid id)
        {
            var response = await _api.PostAsync<FileResponseDto?>("/Chats/GetAttachmentFile", new { Id = id }, true);
            if(response.Result?.FileContent is not null && response.Result.FileContent.Length > 0)
            {
                return File(response.Result.FileContent, response.Result.FileContentType, response.Result.FileNameWithExtension);
            }

            return NotFound();
        }
 
        public async Task<IActionResult> GetAttachmentsFile(Guid id, CancellationToken ct)
        {
            using var raw = await _api.GetRawFileAsync(
                $"/Chats/GetAttachmentFile/{id}",
                authRequired: true,
                cancellationToken: ct);

            if (raw == null)
                return NotFound();

            await using var ms = new MemoryStream();
            await raw.Stream.CopyToAsync(ms, ct);

            if (ms.Length == 0)
                return NotFound(); // proves empty stream

            Response.Headers["Cache-Control"] = "private, max-age=31536000";
            Response.Headers["Content-Disposition"] = "inline";

            return File(
                ms.ToArray(),
                raw.ContentType,
                enableRangeProcessing: true
            );
        }
    }
}
