namespace Wadatika.Application.Features.Teams;

public sealed class TeamOrganizationAccessException(Guid organizationId)
    : Exception(
        $"Organization '{organizationId}' was not found or you do not have access to it.");