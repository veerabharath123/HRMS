using HRMS.SharedKernel.Models.Common.Class;

namespace HRMS.Application.Common.Interface
{
    public interface IJwtTokenServices
    {
        string GenerateToken(JwtUserDto user);
    }
}
