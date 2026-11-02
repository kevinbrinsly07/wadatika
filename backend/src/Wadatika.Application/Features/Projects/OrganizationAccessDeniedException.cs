namespace Wadatika.Application.Features.Projects;

public sealed class OrganizationAccessDeniedException()
    : Exception("You do not have access to this organization.");