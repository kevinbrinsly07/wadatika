using Wadatika.Application.DTOs.Organizations;

namespace Wadatika.Application.Features.Organizations;

public interface IOrganizationService
{
     Task<OrganizationResponse> CreateAsync(
         CreateOrganizationRequest request,
         Guid ownerUserId,
         CancellationToken cancellationToken);

     Task<OrganizationResponse?> GetByIdAsync(
          Guid id,
          Guid userId,
          CancellationToken cancellationToken);
}
