using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.Application.Common.Interface;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Infrastructure.Jwt
{
    public class JwtTokenServices : IJwtTokenServices
    {
        private readonly JwtAuthConfigDto _jwtConfig;
        public JwtTokenServices(IOptions<JwtAuthConfigDto> config)
        {
            _jwtConfig = config.Value;
        }
        public string GenerateToken(JwtUserDto user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SigningKey));
            var encryptKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.EncryptKey));

            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString())
            ];

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenConfig = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                EncryptingCredentials = new EncryptingCredentials(
                    encryptKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler { TokenLifetimeInMinutes = _jwtConfig.ExpiresIn };
            var securityToken = tokenHandler.CreateToken(tokenConfig);
            return tokenHandler.WriteToken(securityToken);
        }

    }
}
