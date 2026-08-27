using AuthService.Application.Common.Behaviors;
using AuthService.Application.Interface;
using AuthService.Infrastructure.Exceptions;
using AuthService.Infrastructure.Persistence.Repositories;
using AuthService.Infrastructure.Security;
using AuthService.Infrastructure.Services;
using FluentValidation;
using System.Reflection;

namespace AuthService.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            return services;
        }
        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký IExceptionHandler
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails(); // Hỗ trợ định dạng lỗi RFC 7807

            return services;
        }
    }
}

