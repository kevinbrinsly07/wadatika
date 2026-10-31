using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wadatika.Application.Interfaces;
using Wadatika.Infrastructure.Persistence;
using Wadatika.Infrastructure.Repositories;

namespace Wadatika.Infrastructure;

public static class DependencyInjection
{
     public static IServiceCollection AddInfrastructure(
         this IServiceCollection services,
         IConfiguration configuration)
     {
          var connectionString =
              configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException(
                  "Database connection string is not configured.");

          services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(connectionString));

          services.AddScoped<
              IOrganizationRepository,
              OrganizationRepository>();

          return services;
     }
}