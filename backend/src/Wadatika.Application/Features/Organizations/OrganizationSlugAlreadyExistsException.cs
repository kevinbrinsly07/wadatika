namespace Wadatika.Application.Features.Organizations;

public sealed class OrganizationSlugAlreadyExistsException(string slug)
     : Exception($"An organization with slug '{slug}' already exists. ");