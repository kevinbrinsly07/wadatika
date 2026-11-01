using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Infrastructure.Authentication;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
     public void Configure(EntityTypeBuilder<RefreshToken> builder)
     {
          builder.ToTable("refresh_tokens");

          builder.HasKey(x => x.Id);

          builder.Property(x => x.TokenHash)
              .IsRequired()
              .HasMaxLength(64);

          builder.Property(x => x.ReplacedByTokenHash)
               .HasMaxLength(64);

          builder.Property(x => x.ExpiresAt)
              .IsRequired();

          builder.Property(x => x.CreatedAt)
              .IsRequired();

          builder.HasIndex(x => x.TokenHash)
              .IsUnique();

          builder.HasIndex(x => x.UserId);

          builder.HasOne<ApplicationUser>()
              .WithMany()
              .HasForeignKey(x => x.UserId)
              .OnDelete(DeleteBehavior.Cascade);
     }
}