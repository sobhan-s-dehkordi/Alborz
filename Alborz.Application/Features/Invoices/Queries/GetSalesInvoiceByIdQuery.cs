using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Invoices.Queries;

public record GetSalesInvoiceByIdQuery(int InvoiceId) : IRequest<SalesInvoiceDetailDto?>;
