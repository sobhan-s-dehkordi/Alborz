using MediatR;

namespace Alborz.Application.Features.Invoices.Queries;

public record GetSalesInvoicesQuery(
    int? CustomerId,
    DateTime? FromDate,
    DateTime? ToDate,
    int? InvoiceId
) : IRequest<List<SalesInvoiceDto>>;
