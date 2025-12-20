using Asp.Versioning;
using HRMS.Application.Services.Chat;
using HRMS.SharedKernel.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IChatServices _chatServices;
        public ChatsController(IChatServices chatServices)
        {
            _chatServices = chatServices;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetChatConversationList()
        {
            var result = await _chatServices.GetChatConversationListAsync();
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> StartNewChatWith([FromBody] IdRequestDto request)
        {
            var result = await _chatServices.StartNewChatWithAsync(request.Id);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetChatConversationDetails([FromBody] IdRequestDto request)
        {
            var result = await _chatServices.GetChatConversationDetailsAsync(request.Id);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequestDto request)
        {
            var result = await _chatServices.SendMessageAsync(request);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> MarkMessageAsDelivered([FromBody] IdRequestDto request)
        {
            var result = await _chatServices.MarkMessageAsDeliveredAsync(request.Id);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> MarkMessageAsRead([FromBody] UpdateSeenRequestDto request)
        {
            var result = await _chatServices.MarkMessageAsReadAsync(request);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendTypingStatus([FromBody] TypingRequestDto request)
        {
            var result = await _chatServices.SendTypingStatusAsync(request);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetPreviousMessages([FromBody] NextMessagesRequestDto request)
        {
            var result = await _chatServices.GetPreviousMessagesAsync(request);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> GetAttachmentFile([FromBody] GuidIdRequestDto request)
        {
            var response = await _chatServices.GetAttachmentFileAsync(request.Id!.Value);
            return Ok(response);
        }
        [HttpGet("[action]/{id:guid}")]
        public async Task<IActionResult> GetAttachmentFile(Guid id, CancellationToken ct)
        {
            var response = await _chatServices.GetAttachmentFileResponseAsync(id);

            if (response is null || response.FileContent is null || response.FileContent.Length == 0)
                return NotFound();

            Response.Headers["Cache-Control"] = "private, max-age=31536000";

            return File(
                response.FileContent,
                response.FileContentType,
                enableRangeProcessing: true
            );
        }
    }
}
