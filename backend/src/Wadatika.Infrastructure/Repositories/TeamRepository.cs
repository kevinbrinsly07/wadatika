using Microsoft.EntityFrameworkCore;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Repositories;

public sealed class TeamRepository(AppDbContext dbContext)
    : ITeamRepository
{
     public Task<bool> IsOrganizationMemberAsync(
         Guid organizationId,
         Guid userId,
         CancellationToken cancellationToken)
     {
          return dbContext.OrganizationMember
              .AsNoTracking()
              .AnyAsync(
                  member =>
                      member.OrganizationId == organizationId &&
                      member.UserId == userId,
                  cancellationToken);
     }

     public async Task AddAsync(
         Team team,
         TeamMember teamLead,
         CancellationToken cancellationToken)
     {
          await dbContext.Team.AddAsync(team, cancellationToken);
          await dbContext.TeamMember.AddAsync(teamLead, cancellationToken);

          await dbContext.SaveChangesAsync(cancellationToken);
     }

     public async Task<IReadOnlyList<Team>> GetByOrganizationForMemberAsync(
     Guid organizationId,
     Guid userId,
     CancellationToken cancellationToken)
     {
          return await dbContext.Team
              .AsNoTracking()
              .Where(team =>
                  team.OrganizationId == organizationId &&
                  dbContext.OrganizationMember.Any(member =>
                      member.OrganizationId == organizationId &&
                      member.UserId == userId))
              .OrderBy(team => team.Name)
              .ToListAsync(cancellationToken);
     }
}