using Wadatika.Application.DTOs.Teams;

namespace Wadatika.Application.Features.Teams;

public interface ITeamService
{
     Task<TeamResponse> CreateAsync(
         Guid organizationId,
         CreateTeamRequest request,
         Guid userId,
         CancellationToken cancellationToken);

     Task<IReadOnlyList<TeamResponse>> GetByOrganizationAsync(
          Guid organizationId,
          Guid userId,
          CancellationToken cancellationToken);
}