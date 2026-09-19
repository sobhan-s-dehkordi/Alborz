using MediatR;

namespace Alborz.Application.Features.Reports.Queries;

public sealed record GetReportQuery(ReportKind Kind, DateTime? From, DateTime? To, int? ProductId)
    : IRequest<IReadOnlyList<ReportRow>>;
