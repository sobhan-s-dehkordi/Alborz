using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Reports.Queries;

public sealed class GetReportQueryHandler(IReportRepository repository)
    : IRequestHandler<GetReportQuery, IReadOnlyList<ReportRow>>
{
    public Task<IReadOnlyList<ReportRow>> Handle(GetReportQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Kind)) throw new ArgumentException("Unknown report.");
        if (request.From?.Date > request.To?.Date) throw new ArgumentException("Start date must precede end date.");
        if (request.Kind == ReportKind.InventoryLedger && request.ProductId is not > 0)
            throw new ArgumentException("Enter a product ID for the inventory ledger.");
        return repository.GetAsync(request, cancellationToken);
    }
}
