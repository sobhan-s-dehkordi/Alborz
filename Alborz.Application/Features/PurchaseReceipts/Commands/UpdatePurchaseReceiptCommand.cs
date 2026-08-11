using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public record UpdatePurchaseReceiptCommand(
    int Id,
    int SupplierId,
    DateTime ReceiptDate,
    string ReferenceNumber,
    decimal TotalDiscount,
    decimal AdditionalCharges,
    string Remarks,
    List<PurchaseItemDto> Items
) : IRequest;
