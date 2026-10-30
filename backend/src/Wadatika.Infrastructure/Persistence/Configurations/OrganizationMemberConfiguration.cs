using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class OrganizationMemberConfiguration
     : IEntityTypeConfiguration<OrganizationMember>
{
     public void Configure(EntityTypeBuilder<OrganizationMember> builder)
     {
          builder.ToTable("organization_members");

          builder.HasKey(member => member.Id);

          builder.Property(member => member.Role)
              .HasConversion<string>()
              .HasMaxLength(32)
              .IsRequired();

          builder.Property(member => member.JoinedAt)
              .HasColumnType("timestamp with time zone")
              .IsRequired();

          builder.HasIndex(member => new

          {
               member.OrganizationId,
               member.UserId
          })
          .IsUnique();

          builder.HasOne<Organization>()
               .WithMany()
               .HasForeignKey(member => member.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);

     }
}