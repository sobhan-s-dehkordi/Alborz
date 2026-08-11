using Alborz.Application.Contracts;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public class GetPurchaseReceiptByIdQueryHandler : IRequestHandler<GetPurchaseReceiptByIdQuery, PurchaseReceiptDto?>
{
    private readonly IPurchaseReceiptRepository _repository;

    public GetPurchaseReceiptByIdQueryHandler(IPurchaseReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseReceiptDto?> Handle(GetPurchaseReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await _repository.GetByIdWithItemsAsync(request.Id);

        if (receipt == null) return null;

        return new PurchaseReceiptDto(
            Id: receipt.Id,
            SupplierId: receipt.PartyId,
            SupplierName: receipt.Party?.Name ?? "Unknown",
            ReceiptDate: receipt.ReceiptDate,
            ReferenceNumber: receipt.ReferenceNumber,
            TotalAmount: receipt.TotalAmount,
            TotalDiscount: receipt.TotalDiscount,
            AdditionalCharges: receipt.AdditionalCharges,
            NetAmount: (receipt.TotalAmount - receipt.TotalDiscount) + receipt.AdditionalCharges,
            Remarks: receipt.Remarks,
            Items: receipt.Items.Select(i => new PurchaseReceiptItemDto(
                Id: i.Id,
                ProductId: i.ProductId,
                ProductName: i.Product?.Name ?? "Unknown",
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice,
                DiscountAmount: i.DiscountAmount,
                TotalPrice: (i.Quantity * i.UnitPrice) - i.DiscountAmount
            )).ToList()
        );
    }
}