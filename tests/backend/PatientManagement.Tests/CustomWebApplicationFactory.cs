using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace PatientManagement.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"pm-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}",
                ["Jwt:Secret"] = "test-only-signing-key-for-integration-tests-1234567890",
                ["Jwt:Issuer"] = "PatientManagement.Api",
                ["Jwt:Audience"] = "PatientManagement.Client",
                ["Jwt:ExpiryMinutes"] = "60",
                ["Cors:AllowedOrigins:0"] = "http://localhost:4200"
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }
}
