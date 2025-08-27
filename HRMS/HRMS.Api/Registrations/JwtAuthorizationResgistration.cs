
using System.Text;
using HRMS.SharedKernel.Models.Common.Class;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Api.Registrations
{
    public class JwtAuthorizationResgistration : IWebApplicationBuilderRegistration
    {
        public void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => ApplyJwtBearerOptions(x, builder.Configuration));
        }

        private static void ApplyJwtBearerOptions(JwtBearerOptions options, ConfigurationManager configuration)
        {
            var config = configuration.GetSection("JwtAuth").Get<JwtAuthConfigDto>()
                            ?? throw new NullReferenceException("JwtAuth configuration is not set.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SigningKey));
            var encryptKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.EncryptKey));

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidIssuer = config.Issuer,
                ValidAudience = config.Audience,
                ClockSkew = TimeSpan.Zero,
                TokenDecryptionKey = encryptKey
            };
        }
    }
}
