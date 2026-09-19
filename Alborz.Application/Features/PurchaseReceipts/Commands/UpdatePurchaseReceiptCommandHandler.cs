using Alborz.Application.Features.PurchaseReceipts.Services;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public sealed class UpdatePurchaseReceiptCommandHandler(PurchaseReceiptWriter writer) : IRequestHandler<UpdatePurchaseReceiptCommand>
{
    public async Task Handle(UpdatePurchaseReceiptCommand request, CancellationToken cancellationToken) =>
        await writer.SaveAsync(request.Id, request.SupplierId, request.ReceiptDate, request.ReferenceNumber,
            request.TotalDiscount, request.AdditionalCharges, request.Remarks, request.Items, cancellationToken);
}
