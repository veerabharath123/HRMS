using Asp.Versioning;
using HRMS.Application.Services;
using HRMS.SharedKernel.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers.V1
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetUsers()
        {
            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                list.Add(i.ToString());
            }
            return Ok(await Task.FromResult(list));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertUser([FromBody] UserInsertRequestDto request)
        {
            var response = await _userServices.InsertUserAsync(request);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> SignUpUser([FromBody] UserInsertRequestDto request)
        {
            var response = await _userServices.SignUpUserAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _userServices.ValidateUserLoginAsync(request);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> GetDocument()
        {
            var response = await _userServices.GetDocument();
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImage([FromBody] FileRequestDto request)
        {
            var response = await _userServices.UploadImage(request);
            return Ok(response);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageByUser([FromBody] MessageRequestDto request)
        {
            var response = await _userServices.SendMessageByUser(request);
            return Ok(response);
        }
    }
}
