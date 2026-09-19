namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public record PurchaseReceiptItemDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountAmount,
    decimal TotalPrice
);

