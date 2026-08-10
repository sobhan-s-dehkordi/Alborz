using Alborz.Application.Contracts;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public class GetPurchaseReceiptsQueryHandler : IRequestHandler<GetPurchaseReceiptsQuery, List<PurchaseReceiptDto>>
{
    private readonly IPurchaseReceiptRepository _repository;

    public GetPurchaseReceiptsQueryHandler(IPurchaseReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PurchaseReceiptDto>> Handle(GetPurchaseReceiptsQuery request, CancellationToken cancellationToken)
    {
        var receipts = await _repository.GetReceiptsAsync(
            request.SupplierId,
            request.FromDate,
            request.ToDate,
            request.ReferenceNumber);

        return receipts.Select(r => new PurchaseReceiptDto(
            Id: r.Id,
            SupplierId: r.PartyId,
            SupplierName: r.Party?.Name ?? "Unknown",
            ReceiptDate: r.ReceiptDate,
            ReferenceNumber: r.ReferenceNumber,

            TotalAmount: r.TotalAmount,
            TotalDiscount: r.TotalDiscount,
            AdditionalCharges: r.AdditionalCharges,
            NetAmount: (r.TotalAmount - r.TotalDiscount) + r.AdditionalCharges,
            Remarks: r.Remarks,

            Items: r.Items.Select(i => new PurchaseReceiptItemDto(
                Id: i.Id,
                ProductId: i.ProductId,
                ProductName: i.Product?.Name ?? "Unknown",
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice,
                DiscountAmount: i.DiscountAmount,
                TotalPrice: (i.Quantity * i.UnitPrice) - i.DiscountAmount
            )).ToList()
        )).ToList();
    }
}
