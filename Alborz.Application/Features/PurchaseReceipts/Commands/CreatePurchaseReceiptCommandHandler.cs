using Alborz.Application.Features.PurchaseReceipts.Services;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public sealed class CreatePurchaseReceiptCommandHandler(PurchaseReceiptWriter writer) : IRequestHandler<CreatePurchaseReceiptCommand, int>
{
    public Task<int> Handle(CreatePurchaseReceiptCommand request, CancellationToken cancellationToken) =>
        writer.SaveAsync(null, request.PartyId, request.ReceiptDate, request.ReferenceNumber,
            request.TotalDiscount, request.AdditionalCharges, request.Remarks, request.Items, cancellationToken);
}
