namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public record PurchaseItemDto(int ProductId, int Quantity, decimal UnitPrice, decimal DiscountAmount);
