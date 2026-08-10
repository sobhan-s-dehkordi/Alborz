namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public record PurchaseReceiptDto(
    int Id,
    int SupplierId,
    string SupplierName,
    DateTime ReceiptDate,
    string ReferenceNumber,
    decimal TotalAmount,
    decimal TotalDiscount,
    decimal AdditionalCharges,
    decimal NetAmount,
    string Remarks,
    List<PurchaseReceiptItemDto> Items
);
