using HRMS.Application.Common.Class;
using HRMS.Application.Common.Interface;
using HRMS.Application.Common.Utitlities;
using HRMS.Domain.Common;
using HRMS.Domain.Constants;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using static HRMS.Domain.Records.UserRecords;

namespace HRMS.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenServices _jwtTokenServices;
        private readonly JwtAuthConfigDto _jwtConfig;
        private readonly IMemoryCache _cache;
        private readonly IDocumentGenerator _documentGenerator;
        public UserServices(IUnitOfWork unitOfWork, IJwtTokenServices jwtTokenServices, IOptions<JwtAuthConfigDto> jwtConfig, IMemoryCache cache, IDocumentGenerator documentGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenServices = jwtTokenServices;
            _jwtConfig = jwtConfig.Value;
            _cache = cache;
            _documentGenerator = documentGenerator;
        }

        private async Task<bool> CheckUserExistAsync(string username)
        {
            return await _unitOfWork.UserRepo.TableNoTracking.AnyAsync(x => x.UserName == username && !x.IsDeleted);
        }
        public async Task<ApiResponseDto<bool>> InsertUserAsync(UserInsertRequestDto request)
        {
            var userExist = await CheckUserExistAsync(request.UserName);

            if (userExist) return ApiResponseDto<bool>.FailureStatus(GeneralConstants.USER_ALREADY_EXISTS_MSG, request.UserName);

            var user = new User();

            user.Add(new UserAddOrUpdateRec(
                string.Empty,
                string.Empty,
                string.Empty,
                request.UserName,
                true
            ));

            SetPassword(user, request.Password);

            _unitOfWork.UserRepo.Add(user);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto<bool>.SuccessStatus(saved);
        }
        private static void SetPassword(User user, string password, bool isUpdate = default)
        {
            if (isUpdate && PasswordHasher.VerifyPasswordHash(password, user.Password, user.HashSalt))
                return;

            PasswordHasher.CreatePasswordHash(password, out byte[] hashedPassword, out byte[] salt);
            user.SetPassword(hashedPassword, salt);
        }
        private async Task<User?> GetUnqiueUserByUserNameAsync(string username)
        {
            return await _unitOfWork.UserRepo.TableNoTracking
                .SingleOrDefaultAsync(x => !x.IsDeleted && x.UserName.ToLower().Equals(username.ToLower()));
        }

        public async Task<ApiResponseDto<LoginResponseDto>> ValidateUserLoginAsync(LoginRequestDto request)
        {
            var user = await GetUnqiueUserByUserNameAsync(request.UserName);

            if (user is null)
                return ApiResponseDto<LoginResponseDto>.FailureStatus("User {0} does not exists", request.UserName);

            var isValid = PasswordHasher.VerifyPasswordHash(request.Password, user.Password, user.HashSalt);

            if (isValid) return await CreateLoginResponseAsync(user);

            return ApiResponseDto<LoginResponseDto>.FailureStatus("Incorrect password/username");
        }
        private async Task<ApiResponseDto<LoginResponseDto>> CreateLoginResponseAsync(User user)
        {
            var loginResponse = new LoginResponseDto
            {
                Permissions = await GetPermissionsByUserIdAsync(user.Id),
                Roles = await GetRolesByUserIdAsync(user.Id),
                UserId = user.GuidId,
                UserName = user.UserName,
                TokenExpiry = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiresIn)
            };

            loginResponse.Token = _jwtTokenServices.GenerateToken(loginResponse);
            _cache.Set($"permissions_{user.Id}", loginResponse.Permissions, TimeSpan.FromMinutes(15));

            return ApiResponseDto<LoginResponseDto>.SuccessStatus(loginResponse);
        }
        public async Task<List<string>> GetPermissionsByUserIdAsync(Guid Id)
        {
            var userId = await _unitOfWork.UserRepo.GetIdByGuid(Id);
            if (userId is null) return [];
            return await GetPermissionsByUserIdAsync(userId.Value);
        }

        private async Task<List<string>> GetPermissionsByUserIdAsync(int userId)
        {
            var result = await (from u in _unitOfWork.UserRolesRepo.TableNoTracking
                                join rp in _unitOfWork.RolePermissionsRepo.TableNoTracking on u.RoleId equals rp.RoleId
                                join p in _unitOfWork.PermissionsRepo.TableNoTracking on rp.PermissionId equals p.Id
                                where u.UserId == userId && !u.IsDeleted && !p.IsDeleted && !rp.IsDeleted
                                select p.Name).ToListAsync();

            return result;
        }
        public async Task<List<string>> GetRolesByUserIdAsync(int userId)
        {
            var result = await (from u in _unitOfWork.UserRolesRepo.TableNoTracking
                                join r in _unitOfWork.RolesRepo.TableNoTracking on u.RoleId equals r.Id
                                where u.UserId == userId && !u.IsDeleted && !r.IsDeleted
                                select r.Name).ToListAsync();

            return result;
        }
        public async Task<ApiResponseDto<FileResponseDto>> GetDocument()
        {
            var filename = "Sample Template";
            var username = "Venkat";

            var fields = new SampleDocDto
            {
                Name = "Bharath",
                ToName = username,
                IssuedBy = "Unknown",
                DateOfBirth = DateTime.Now.ToString("yyyy-MM-dd"),
                RegistrationDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                Title = "Sample Test Document",
            };

            var signaturePath = string.Format(GeneralConstants.SAMPE_SIGNATURE_PATH_PNG, AppDomain.CurrentDomain.BaseDirectory, username);
            var templatePath = string.Format(GeneralConstants.SAMPE_TEMPLATE_PATH, AppDomain.CurrentDomain.BaseDirectory);

            var docTemplate = DocTemplateBuilder
                        .Create()
                        .WithTextFromModel(fields)
                        .WithText("para1", GeneralConstants.WORD_SAMPLE_PARA)
                        .WithText("style", "test text", new() { FontSize = 32, Bold = true, Italic = true ,FontName = "French Script MT" })
                        .WithImage("Signature", signaturePath, new(100, 40))
                        .WithTable("table", [
                            ["Header1", "Header2", "Header3"],
                            ["Row1 Col1", "Row1 Col2", "Row1 Col3"],
                            ["Row2 Col1", "Row2 Col2", "Row2 Col3"],
                            ["Row3 Col1", "Row3 Col2", "Row3 Col3"]
                            ])
                        .Build();

            var bytes = _documentGenerator.GenerateDocument(templatePath, docTemplate, GeneralConstants.DocumentType.Word);
            
            var response = new FileResponseDto
            {
                FileName = filename,
                FileExtension = "docx",
                FileContent = bytes,
                FileContentType = GeneralConstants.CONTENT_TYPE_DOCX
            };

            
            return await Task.FromResult(ApiResponseDto<FileResponseDto>.SuccessStatus(response));
        }
    }
}
