using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class TeamConfiguration
    : IEntityTypeConfiguration<Team>
{
     public void Configure(EntityTypeBuilder<Team> builder)
     {
          builder.ToTable("teams");

          builder.HasKey(team => team.Id);

          builder.Property(team => team.Name)
              .HasMaxLength(100)
              .IsRequired();

          builder.Property(team => team.CreatedAt)
              .HasColumnType("timestamp with time zone")
              .IsRequired();

          builder.Property(team => team.UpdateAt)
              .HasColumnType("timestamp with time zone");

          builder.HasIndex(team => new
          {
               team.OrganizationId,
               team.Name
          });

          builder.HasOne<Organization>()
              .WithMany()
              .HasForeignKey(team => team.OrganizationId)
              .OnDelete(DeleteBehavior.Cascade);
     }
}