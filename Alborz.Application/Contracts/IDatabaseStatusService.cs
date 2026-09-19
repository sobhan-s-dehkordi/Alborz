namespace Alborz.Application.Contracts;

public interface IDatabaseStatusService
{
    string Server { get; }
    string Database { get; }
    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
