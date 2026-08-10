using Alborz.Application.Features.PurchaseReceipts.Commands;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public record GetPurchaseReceiptsQuery(
    int? SupplierId,
    DateTime? FromDate,
    DateTime? ToDate,
    string ReferenceNumber
) : IRequest<List<PurchaseReceiptDto>>;
