using System.Text;
using AuthService.Application.Interface;
using AuthService.Infrastructure.Persistence.Authentication;
using AuthService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
namespace AuthService.Application.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // 1. Bind cấu hình JwtSettings
            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            // 2. Đăng ký Generator Service
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // 3. Cấu hình middleware xác thực JwtBearer
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Hết hạn chính xác không có độ trễ mặc định (5 phút)
                };
            });

            services.AddAuthorization();

            return services;
        }
    }
}
