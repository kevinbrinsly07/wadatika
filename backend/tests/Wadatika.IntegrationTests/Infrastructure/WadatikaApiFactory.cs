using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Wadatika.UnitTests.Integration;

public sealed class WadatikaApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("WADATIKA_TEST_CONNECTION")
            ?? throw new InvalidOperationException(
                "WADATIKA_TEST_CONNECTION environment variable is required.");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connectionString,
                ["Frontend:Origin"] = "http://localhost:3000"
            };

            configuration.AddInMemoryCollection(settings);
        });

        builder.UseEnvironment("Testing");
    }
}