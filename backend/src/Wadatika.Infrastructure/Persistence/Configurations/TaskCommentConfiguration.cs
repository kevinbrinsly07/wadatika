using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Authentication;

namespace Wadatika.Infrastructure.Persistence.Configurations;

public sealed class TaskCommentConfiguration
    : IEntityTypeConfiguration<TaskComment>
{
    public void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        builder.ToTable("task_comments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Content)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(comment => comment.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        // A task can have multiple comments.
        builder.HasOne<ProjectTask>()
            .WithMany()
            .HasForeignKey(comment => comment.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Preserve comments rather than silently deleting them
        // when an author account is removed.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(comment => comment.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(comment => new
        {
            comment.ProjectTaskId,
            comment.CreatedAt
        });

        builder.HasIndex(comment => comment.AuthorUserId);
    }
}