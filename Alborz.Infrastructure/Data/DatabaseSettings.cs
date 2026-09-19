namespace Alborz.Infrastructure.Data;

public static class DatabaseSettings
{
    public const string EnvironmentVariable = "ALBORZ_SQLSERVER_CONNECTION";
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable(EnvironmentVariable) is { Length: > 0 } value
            ? value
            : @"Data Source=.;Initial Catalog=Alborz;user Id=sa;Password=3508;TrustServerCertificate=true;";
}
