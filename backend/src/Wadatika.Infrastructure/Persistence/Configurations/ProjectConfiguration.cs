using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration
    : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(project => project.Key)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(500);

        builder.Property(project => project.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(project => project.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // A project key must be unique within its organization.
        builder.HasIndex(project => new
        {
            project.OrganizationId,
            project.Key
        })
        .IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(project => project.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}