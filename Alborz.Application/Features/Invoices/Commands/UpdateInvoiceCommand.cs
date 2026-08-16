using Alborz.Domain.Enums;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public record UpdateInvoiceCommand(
    int Id,
    int? CustomerId,
    PaymentMethod PaymentMethod,
    decimal GlobalDiscount,
    decimal AdditionalCharges,
    string Remarks,
    List<InvoiceItemDto> Items
) : IRequest;
