using Alborz.Domain.Enums;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public record CreateInvoiceCommand(
    int? CustomerId,
    PaymentMethod PaymentMethod,
    decimal GlobalDiscount,
    decimal AdditionalCharges,
    string Remarks,
    List<InvoiceItemDto> Items, DateTime? InvoiceDate = null) : IRequest<int>;

