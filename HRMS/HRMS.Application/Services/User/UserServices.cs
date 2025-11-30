using HRMS.Application.Common.Class;
using HRMS.Application.Common.Class.DocTemplateBuilder;
using HRMS.Application.Common.Class.LinqExtensions;
using HRMS.Application.Common.Interface;
using HRMS.Application.Common.Utitlities;
using HRMS.Application.Services.CommonFunctions;
using HRMS.Domain.Common;
using HRMS.Domain.Constants;
using HRMS.Domain.Entites;
using HRMS.SharedKernel.Models.Common;
using HRMS.SharedKernel.Models.Common.Class;
using HRMS.SharedKernel.Models.Request;
using HRMS.SharedKernel.Models.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Security.Claims;
using static HRMS.Domain.Records.UserRecords;

namespace HRMS.Application.Services
{
    public class UserServices : CommonFunctionsSerivces, IUserServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenServices _jwtTokenServices;
        private readonly JwtAuthConfigDto _jwtConfig;
        private readonly IMemoryCache _cache;
        private readonly IDocumentGenerator _documentGenerator;
        private readonly ISystemNotificationServices _systemNotificationServices;
        private readonly IChatServices _chatServices;
        private readonly IEmailServices _emailServices;
        public UserServices(IUnitOfWork unitOfWork, IJwtTokenServices jwtTokenServices, IOptions<JwtAuthConfigDto> jwtConfig, IMemoryCache cache, IDocumentGenerator documentGenerator,
            ISystemNotificationServices systemNotificationServices,IHttpContextAccessor httpContextAccessor, IChatServices chatServices,
            IEmailServices emailServices)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenServices = jwtTokenServices;
            _jwtConfig = jwtConfig.Value;
            _cache = cache;
            _documentGenerator = documentGenerator;
            _systemNotificationServices = systemNotificationServices;
            _httpContextAccessor = httpContextAccessor;
            _chatServices = chatServices;
            _emailServices = emailServices;
        }
        public async Task<ApiResponseDto> GetUsersAsync()
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid userId))
            {
                return ApiResponseDto.FailureStatus("Invalid user ID");
            }
            
            var chatUsers = await _unitOfWork.UserRepo.TableNoTracking
                .Where(x => !x.IsDeleted && x.GuidId != userId && x.IsActive)
                .Select(x => new ChatUserResponseDto
                {
                    UserId = x.GuidId,
                    UserName = x.UserName
                }).ToListAsync();

            return ApiResponseDto.SuccessStatus(chatUsers);
        }
        private async Task<bool> CheckUserExistAsync(string username)
        {
            return await _unitOfWork.UserRepo.TableNoTracking.AnyAsync(x => x.UserName == username && !x.IsDeleted);
        }
        public async Task<ApiResponseDto> InsertUserAsync(UserInsertRequestDto request)
        {
            var userExist = await CheckUserExistAsync(request.UserName);

            if (userExist) return ApiResponseDto.FailureStatus(GeneralConstants.USER_ALREADY_EXISTS_MSG, request.UserName);

            var user = new User();

            user.Add(new UserAddOrUpdateRec(
                request.Email,
                request.UserName
            ));

            SetPassword(user, request.Password);

            _unitOfWork.UserRepo.Add(user);
            var saved = await _unitOfWork.SaveAsync();

            return ApiResponseDto.SuccessStatus(saved);
        }
        public async Task<ApiResponseDto> SignUpUserAsync(UserInsertRequestDto request)
        {
            var insertRes = await InsertUserAsync(request);
            if (!insertRes.Success) return ApiResponseDto.FailureStatus(insertRes.Message);

            return await ValidateUserLoginAsync(request);
        }
        private static void SetPassword(User user, string password, bool isUpdate = default)
        {
            if (isUpdate && PasswordHasher.VerifyPasswordHash(password, user.Password, user.HashSalt))
                return;

            PasswordHasher.CreatePasswordHash(password, out byte[] hashedPassword, out byte[] salt);
            user.SetPassword(hashedPassword, salt);
        }
        private  Task<User?> GetUnqiueUserByUserNameAsync(string username)
        {
            return _unitOfWork.UserRepo.TableNoTracking
                .SingleOrDefaultAsync(x => !x.IsDeleted && x.UserName.ToLower().Equals(username.ToLower()));
        }

        public async Task<ApiResponseDto> ValidateUserLoginAsync(LoginRequestDto request)
        {
            var user = await GetUnqiueUserByUserNameAsync(request.UserName);

            if (user is null)
                return ApiResponseDto.FailureStatus("User {0} does not exists", request.UserName);

            var isValid = PasswordHasher.VerifyPasswordHash(request.Password, user.Password, user.HashSalt);

            //await _emailServices.SendMailAsync(new EmailMessageDto
            //{
            //    To = [new EmailAddressDto { EmailAddress = user.Email, DisplayName = user.UserName }],
            //    Subject = "Login Alert",
            //    Body = $"<p>Hi {user.UserName},</p><p>Your account was just accessed on {DateTime.UtcNow} UTC. If this was not you, please reset your password immediately or contact support.</p><p>Thank you,<br/>HRMS Team</p>",
            //    IsHtml = true
            //});

            if (isValid) return await CreateLoginResponseAsync(user);

            return ApiResponseDto.FailureStatus("Incorrect password/username");
        }
        private async Task<ApiResponseDto> CreateLoginResponseAsync(User user)
        {
            var loginResponse = new LoginResponseDto
            {
                Permissions = await GetPermissionsByUserIdAsync(user.Id),
                Roles = await GetRolesByUserIdAsync(user.Id),
                UserId = user.Id,
                UserName = user.UserName,
                TokenExpiry = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiresIn)
            };

            loginResponse.Token = _jwtTokenServices.GenerateToken(loginResponse);
            _cache.Set($"permissions_{user.Id}", loginResponse.Permissions, TimeSpan.FromMinutes(15));

            return ApiResponseDto.SuccessStatus(loginResponse);
        }

        public async Task<List<string>> GetPermissionsByUserIdAsync(int userId)
        {
            return await (from u in _unitOfWork.UserRolesRepo.TableNoTracking
                                join rp in _unitOfWork.RolePermissionsRepo.TableNoTracking on u.RoleId equals rp.RoleId
                                join p in _unitOfWork.PermissionsRepo.TableNoTracking on rp.PermissionId equals p.Id
                                where u.UserId == userId && !u.IsDeleted && !p.IsDeleted && !rp.IsDeleted
                                select p.Name).ToListAsync();
        }
        public Task<List<string>> GetRolesByUserIdAsync(int userId)
        {
            return (from u in _unitOfWork.UserRolesRepo.TableNoTracking
                                join r in _unitOfWork.RolesRepo.TableNoTracking on u.RoleId equals r.Id
                                where u.UserId == userId && !u.IsDeleted && !r.IsDeleted
                                select r.Name).ToListAsync();

        }
        public async Task<ApiResponseDto> GetDocument()
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

            List<UserInsertRequestDto> data = [
                    new() { UserName = "Bharath", Email = "bbb" },
                    new() { UserName = "Venkat", Email = "vvv" },
                    new() { UserName = "Ankit", Email = "vvv" },
                ];

            var signaturePath = string.Format(GeneralConstants.SAMPE_SIGNATURE_PATH_PNG, AppDomain.CurrentDomain.BaseDirectory, username);
            var templatePath = string.Format(GeneralConstants.SAMPE_TEMPLATE_PATH, AppDomain.CurrentDomain.BaseDirectory);

            var docTemplate = DocTemplateBuilder
                                .Create()
                                .WithTextFromModel(fields)
                                .WithImage("Signature", signaturePath, new() { Width = 100, Height = 40 })
                                .WithTableFromModel<UserInsertRequestDto>(
                                    t => t.ConfigureTable("table",data)
                                            .AddColumn("User Name", x => x.UserName, 
                                                    s => s.UserName == "Bharath" ? new() { Color = "dodgerblue" } : null)
                                            .AddColumn("Email", x => x.Email)
                                )
                                .Build();

            var bytes = _documentGenerator.GenerateWordDocument(templatePath, docTemplate);
            
            var response = new FileResponseDto
            {
                FileName = filename,
                FileExtension = "docx",
                FileContent = bytes,
                FileContentType = GeneralConstants.CONTENT_TYPE_DOCX
            };
            var user =  await GetUnqiueUserByUserNameAsync("Bharath");
            if(user is not null)
                await _systemNotificationServices.SendNotificationAsync(user.GuidId.ToString(),"hello", "Test");

            return await Task.FromResult(ApiResponseDto.SuccessStatus(response));
        }

        public async Task<ApiResponseDto> UploadImage(FileRequestDto request)
        {
            //if (FileValidator.Validate(request))
            //{
            //    return ApiResponseDto<bool>.FailureStatus("Invalid file");
            //}

            var ftpConfig = new FtpConfigDto
            {
                FtpBaseUrl = "hrmsftp.local",
                FtpUsername = "VeeraBharath",
                FtpPassword = "ftppswd001"
            };

            var remotePath = $"{Path.GetFileNameWithoutExtension(request.FileName)}|{Guid.NewGuid()}.{Path.GetExtension(request.FileName)}";

            using var stream = new MemoryStream(request.FileContent!);

            var isUploaded = false;// await _ftpFileServices.UploadAsync(remotePath, stream, ftpConfig);

            return await Task.FromResult(ApiResponseDto.SuccessStatus(isUploaded));
        }
        public async Task<ApiResponseDto> SendMessageByUser(MessageRequestDto request)
        {
            var response = new MessageResponseDto
            {
                Date = request.Date,
                Message = request.Message,
                IsMe = false,
                Username = GetCurrentUsername()
            };

            await _chatServices.SendMessageToUserAsync(request.UserId.ToString(), response);

            //TODO: Save message to database

            return ApiResponseDto.FlagStatus(true, "Message sent");

        }
    }
}
