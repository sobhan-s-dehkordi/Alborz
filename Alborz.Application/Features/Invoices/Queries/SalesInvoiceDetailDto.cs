using Alborz.Domain.Enums;

namespace Alborz.Application.Features.Invoices.Queries;

public record SalesInvoiceDetailDto(
    int Id,
    int? CustomerId,
    string? CustomerName,
    string? CustomerPhone,
    DateTime InvoiceDate,
    PaymentMethod PaymentMethod,
    string Remarks,
    decimal GlobalDiscount,
    decimal AdditionalCharges,
    List<SalesInvoiceItemDetailDto> Items)
{
    public decimal TotalAmount => Items?.Sum(x => x.TotalPrice) ?? 0;
    public decimal NetAmount => TotalAmount - GlobalDiscount + AdditionalCharges;
};
