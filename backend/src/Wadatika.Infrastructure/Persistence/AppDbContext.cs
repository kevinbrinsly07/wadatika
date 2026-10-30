using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Wadatika.Domain.Entities;

namespace Wadatika.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
     : DbContext(options)
{
     public DbSet<Organization> Organizations => Set<Organization>();
     public DbSet<OrganizationMember> OrganizationMember => Set<OrganizationMember>();
     public DbSet<Team> Team => Set<Team>();
     public DbSet<TeamMember> TeamMember => Set<TeamMember>();

     protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
          base.OnModelCreating(modelBuilder);

          modelBuilder.ApplyConfigurationsFromAssembly(
               typeof(AppDbContext).Assembly);
     }

}