using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Authentication;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class ProjectTaskConfiguration
    : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("project_tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasMaxLength(2000);

        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(task => task.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(task => task.DueDate)
            .HasColumnType("date");

        builder.Property(task => task.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(task => task.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // A project contains many tasks.
        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Preserve tasks if their creator's account is deleted.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(task => task.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // An assignee is optional.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(task => task.AssigneeUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Useful for project task lists and filters.
        builder.HasIndex(task => new
        {
            task.ProjectId,
            task.Status
        });

        builder.HasIndex(task => task.AssigneeUserId);
        builder.HasIndex(task => task.DueDate);
    }
}