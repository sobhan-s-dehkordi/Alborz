namespace Alborz.Application.Features.Invoices.Queries;

public record SalesInvoiceDto(
    int Id,
    DateTime InvoiceDate,
    string CustomerName,
    string Remarks,
    decimal TotalAmount,
    decimal TotalDiscount,
    decimal AdditionalCharges,
    decimal NetAmount
);
