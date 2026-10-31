using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Wadatika.Application.Features.Organizations;

namespace Wadatika.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            IOrganizationService,
            OrganizationService>();

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        return services;
    }
}