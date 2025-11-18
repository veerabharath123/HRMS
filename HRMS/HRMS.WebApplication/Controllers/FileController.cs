using HRMS.SharedKernel.Models.Request;
using HRMS.WebApplication.Class;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.WebApplication.Controllers
{
    public class FileController : BaseController
    {
        private readonly ApiRequest _api;
        public FileController(ApiRequest api)
        {
            _api = api;
        }
        [HttpPost]
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            var request = await ToFileDtoAsync(file);
            var response = await _api.PostAsync<Guid>("Files/UploadFileAsync", request, true);
            return JsonResponse(response);
        }
        public static async Task<object> ToFileDtoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            return new
            {
                FileContent = ms.ToArray(),
                FileName = Path.GetFileNameWithoutExtension(file.FileName),
                FileExtension = Path.GetExtension(file.FileName)?.TrimStart('.') ?? "",
                FileContentType = file.ContentType
            };
        }
    }
}
