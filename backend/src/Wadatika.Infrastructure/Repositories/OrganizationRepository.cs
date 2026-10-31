using Microsoft.EntityFrameworkCore;
using Wadatika.Application.Interfaces;
using Wadatika.Domain.Entities;
using Wadatika.Infrastructure.Persistence;

namespace Wadatika.Infrastructure.Repositories;

public sealed class OrganizationRepository(AppDbContext dbContext)
    : IOrganizationRepository
{
     public Task<bool> SlugExistsAsync(
         string slug,
         CancellationToken cancellationToken)
     {
          return dbContext.Organizations
              .AsNoTracking()
              .AnyAsync(
                  organization => organization.Slug == slug,
                  cancellationToken);
     }

     public async Task AddAsync(
         Organization organization,
         CancellationToken cancellationToken)
     {
          await dbContext.Organizations.AddAsync(
              organization,
              cancellationToken);

          await dbContext.SaveChangesAsync(cancellationToken);
     }

     public Task<Organization?> GetByIdAsync(
         Guid id,
         CancellationToken cancellationToken)
     {
          return dbContext.Organizations
              .AsNoTracking()
              .FirstOrDefaultAsync(
                  organization => organization.Id == id,
                  cancellationToken);
     }
}