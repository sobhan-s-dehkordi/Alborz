using Alborz.Application;
using Alborz.Infrastructure;
using Alborz.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alborz.Tests;

public sealed class SqlServerFixture : IAsyncLifetime
{
    public ServiceProvider Services { get; private set; } = null!;
    private string _database = string.Empty;

    public async Task InitializeAsync()
    {
        _database = "Alborz_Tests_" + Guid.NewGuid().ToString("N");
        var connection = new SqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable("ALBORZ_TEST_SQLSERVER_CONNECTION") ?? DatabaseSettings.ConnectionString)
        { InitialCatalog = _database };
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(connection.ConnectionString);
        Services = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (Services is null) return;
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (_database.StartsWith("Alborz_Tests_", StringComparison.Ordinal) && db.Database.GetDbConnection().Database == _database)
                await db.Database.EnsureDeletedAsync();
        }
        await Services.DisposeAsync();
    }
}
