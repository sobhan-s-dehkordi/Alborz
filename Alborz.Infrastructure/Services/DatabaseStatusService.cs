using Alborz.Application.Contracts;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Services;

public sealed class DatabaseStatusService(AppDbContext context) : IDatabaseStatusService
{
    public string Server => context.Database.GetDbConnection().DataSource;
    public string Database => context.Database.GetDbConnection().Database;
    public Task<bool> CanConnectAsync(CancellationToken cancellationToken = default) =>
        context.Database.CanConnectAsync(cancellationToken);
}
