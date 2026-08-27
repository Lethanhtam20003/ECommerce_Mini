using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("USERS");

            builder.HasKey(e => e.Id);
            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()"); // Tự động sinh GUID khi thêm mới

            builder.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
               .HasColumnName("password_hash");
        }
    }
}
