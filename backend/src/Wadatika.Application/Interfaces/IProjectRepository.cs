using Wadatika.Domain.Entities;

namespace Wadatika.Application.Interfaces;

public interface IProjectRepository
{
    Task<bool> IsOrganizationMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> KeyExistsAsync(
        Guid organizationId,
        string key,
        CancellationToken cancellationToken);

    Task AddAsync(
        Project project,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken);
}