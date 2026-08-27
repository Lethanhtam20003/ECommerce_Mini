using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence
{
    /// <summary>
    ///
    /// </summary>

    //TODO: Add a description for the AuthDbContext class.
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Tự động tìm và nạp tất cả Class implement IEntityTypeConfiguration trong Assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);

        }
    }
}
