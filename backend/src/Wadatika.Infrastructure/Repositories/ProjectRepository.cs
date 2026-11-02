using Microsoft.EntityFrameworkCore;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Repositories;

public sealed class ProjectRepository(AppDbContext dbContext)
    : IProjectRepository
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

    public Task<bool> KeyExistsAsync(
        Guid organizationId,
        string key,
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .AsNoTracking()
            .AnyAsync(
                project =>
                    project.OrganizationId == organizationId &&
                    project.Key == key,
                cancellationToken);
    }

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken)
    {
        await dbContext.Projects.AddAsync(
            project,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.OrganizationId == organizationId)
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }
}