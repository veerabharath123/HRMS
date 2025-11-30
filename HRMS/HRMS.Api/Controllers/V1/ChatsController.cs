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
    }
}
