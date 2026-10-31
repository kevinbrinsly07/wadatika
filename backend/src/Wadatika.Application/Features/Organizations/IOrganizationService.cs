using Wadatika.Application.DTOs.Organizations;

namespace Wadatika.Application.Features.Organizations;

public interface IOrganizationService
{
     Task<OrganizationResponse> CreateAsync(
          CreateOrganizationRequest request,
          CancellationToken cancellationToken);
     Task<OrganizationResponse?> GetByIdAsync(
          Guid id,
          CancellationToken cancellationToken);
}