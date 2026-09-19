using Alborz.Application.Features.Reports.Queries;

namespace Alborz.Application.Contracts;

public interface IReportRepository
{
    Task<IReadOnlyList<ReportRow>> GetAsync(GetReportQuery query, CancellationToken cancellationToken);
}
