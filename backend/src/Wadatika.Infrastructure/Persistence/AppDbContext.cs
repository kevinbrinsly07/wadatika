using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Authentication;


namespace Wadatika.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
     : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)

{
     public DbSet<Organization> Organizations => Set<Organization>();
     public DbSet<OrganizationMember> OrganizationMember => Set<OrganizationMember>();
     public DbSet<Team> Team => Set<Team>();
     public DbSet<TeamMember> TeamMember => Set<TeamMember>();
     public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          base.OnModelCreating(modelBuilder);

          modelBuilder.ApplyConfigurationsFromAssembly(
               typeof(AppDbContext).Assembly);
     }

}