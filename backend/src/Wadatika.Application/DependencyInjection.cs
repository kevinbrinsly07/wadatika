using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wadatika.Application.Features.Organizations;
using Wadatika.Application.Features.Teams;
using Wadatika.Application.Features.Projects;

namespace Wadatika.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            IOrganizationService,
            OrganizationService>();

        services.AddScoped<
            ITeamService,
            TeamService>();

        services.AddScoped<
            IProjectService,
            ProjectService>();

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        return services;
    }
}