using Alborz.Domain.Enums;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(
    int? CustomerId,
    PaymentMethod PaymentMethod,
    List<InvoiceItemDto> Items) : IRequest<int>;
