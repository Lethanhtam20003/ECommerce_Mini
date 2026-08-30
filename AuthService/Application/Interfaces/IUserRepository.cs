using AuthService.Domain.Entities;

namespace AuthService.Application.Interface
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<User?> GetUserAsync(string email, bool tracking, CancellationToken cancellationToken);
    }
}
