using AspNetCore.ReCaptcha;
using HRMS.Application.Common.Interface;

namespace HRMS.Infrasturcture.Recaptcha
{
    public class GoogleRecaptchaServices : ICaptchaServices
    {
        private readonly IReCaptchaService _recaptchaService;

        public GoogleRecaptchaServices(IReCaptchaService recaptchaService)
        {
            _recaptchaService = recaptchaService;
        }
        public async Task<bool> VerifyRecaptcha(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            return await _recaptchaService.VerifyAsync(response);
        }
    }
}
