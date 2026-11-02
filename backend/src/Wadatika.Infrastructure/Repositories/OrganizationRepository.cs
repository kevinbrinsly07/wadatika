
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
        OrganizationMember owner,
        CancellationToken cancellationToken)
    {
        await dbContext.Organizations.AddAsync(
            organization,
            cancellationToken);

        await dbContext.OrganizationMember.AddAsync(
            owner,
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

    public Task<Organization?> GetByIdForMemberAsync(
    Guid id,
    Guid userId,
    CancellationToken cancellationToken)
    {
        return dbContext.Organizations
            .AsNoTracking()
            .Where(organization =>
                organization.Id == id &&
                dbContext.OrganizationMember.Any(member =>
                    member.OrganizationId == organization.Id &&
                    member.UserId == userId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Organization>> GetAllForMemberAsync(
    Guid userId,
    CancellationToken cancellationToken)
    {
        return await dbContext.Organizations
            .AsNoTracking()
            .Where(organization =>
                dbContext.OrganizationMember.Any(member =>
                    member.OrganizationId == organization.Id &&
                    member.UserId == userId))
            .OrderBy(organization => organization.Name)
            .ToListAsync(cancellationToken);
    }
}
