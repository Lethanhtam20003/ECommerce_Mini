using AuthService.Domain.Entities;

namespace AuthService.Application.Interface
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
