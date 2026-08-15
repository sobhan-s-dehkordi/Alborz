namespace Alborz.Application.Features.Invoices.Queries;

public record SalesInvoiceItemDetailDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountAmount
);
