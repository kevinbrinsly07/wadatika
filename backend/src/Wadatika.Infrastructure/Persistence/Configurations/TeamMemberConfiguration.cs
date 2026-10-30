using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wadatika.Domain.Entities;

namespace WadAtika.Infrastructure.Persistence.Configurations;

public sealed class TeamMemberConfiguration
    : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("team_members");

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
            member.TeamId,
            member.UserId
        })
        .IsUnique();

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(member => member.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}