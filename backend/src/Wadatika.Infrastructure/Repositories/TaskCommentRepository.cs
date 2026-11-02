using Microsoft.EntityFrameworkCore;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Repositories;

public sealed class TaskCommentRepository(AppDbContext dbContext)
    : ITaskCommentRepository
{
     public Task<bool> IsTaskAccessibleAsync(
     Guid organizationId,
     Guid projectId,
     Guid taskId,
     Guid userId,
     CancellationToken cancellationToken)
     {
          return (
              from task in dbContext.ProjectTasks
              join project in dbContext.Projects
                  on task.ProjectId equals project.Id
              where task.Id == taskId
                    && project.Id == projectId
                    && project.OrganizationId == organizationId
                    && dbContext.OrganizationMember.Any(member =>
                        member.OrganizationId == organizationId &&
                        member.UserId == userId)
              select task.Id
          ).AnyAsync(cancellationToken);
     }

     public async Task AddAsync(
         TaskComment comment,
         CancellationToken cancellationToken)
     {
          await dbContext.TaskComments.AddAsync(comment, cancellationToken);
          await dbContext.SaveChangesAsync(cancellationToken);
     }

     public async Task<IReadOnlyList<TaskComment>> GetByTaskAsync(
         Guid taskId,
         CancellationToken cancellationToken)
     {
          return await dbContext.TaskComments
              .AsNoTracking()
              .Where(comment => comment.ProjectTaskId == taskId)
              .OrderBy(comment => comment.CreatedAt)
              .ToListAsync(cancellationToken);
     }
}