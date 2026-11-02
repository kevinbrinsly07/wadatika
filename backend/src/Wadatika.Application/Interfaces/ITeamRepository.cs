using Wadatika.Domain.Entities;

namespace Wadatika.Application.Interfaces;

public interface ITeamRepository
{
    Task<bool> IsOrganizationMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Team team,
        TeamMember teamLead,
        CancellationToken cancellationToken);
     
     Task<IReadOnlyList<Team>> GetByOrganizationForMemberAsync(
          Guid organizationId,
          Guid userId,
          CancellationToken cancellationToken);
}