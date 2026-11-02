using Microsoft.EntityFrameworkCore;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Repositories;

public sealed class ProjectTaskRepository(AppDbContext dbContext)
    : IProjectTaskRepository
{
    public Task<bool> IsProjectAccessibleAsync(
        Guid organizationId,
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .AsNoTracking()
            .AnyAsync(
                project =>
                    project.Id == projectId &&
                    project.OrganizationId == organizationId &&
                    dbContext.OrganizationMember.Any(
                        member =>
                            member.OrganizationId == project.OrganizationId &&
                            member.UserId == userId),
                cancellationToken);
    }

    public Task<bool> IsProjectMemberAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .AsNoTracking()
            .AnyAsync(
                project =>
                    project.Id == projectId &&
                    dbContext.OrganizationMember.Any(
                        member =>
                            member.OrganizationId == project.OrganizationId &&
                            member.UserId == userId),
                cancellationToken);
    }

    public async Task AddAsync(
        ProjectTask task,
        CancellationToken cancellationToken)
    {
        await dbContext.ProjectTasks.AddAsync(
            task,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectTask>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await dbContext.ProjectTasks
            .AsNoTracking()
            .Where(task => task.ProjectId == projectId)
            .OrderByDescending(task => task.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<ProjectTask?> GetByIdAsync(
        Guid projectId,
        Guid taskId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProjectTasks
            .FirstOrDefaultAsync(
                task =>
                    task.ProjectId == projectId &&
                    task.Id == taskId,
                cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}