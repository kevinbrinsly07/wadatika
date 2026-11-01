using Wadatika.Domain.Entities;

namespace Wadatika.Application.Interfaces;

public interface IOrganizationRepository
{
     Task<bool> SlugExistsAsync(
         string slug,
         CancellationToken cancellationToken);

     Task AddAsync(
         Organization organization,
         OrganizationMember owner,
         CancellationToken cancellationToken);

     Task<Organization?> GetByIdAsync(
         Guid id,
         CancellationToken cancellationToken);

     Task<Organization?> GetByIdForMemberAsync(
          Guid id,
          Guid userId,
          CancellationToken cancellationToken);
}