using AuthService.Domain.Entities;

namespace AuthService.Application.Interface
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
