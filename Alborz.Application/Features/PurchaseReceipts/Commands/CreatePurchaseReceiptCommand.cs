using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public record CreatePurchaseReceiptCommand(string SupplierName, List<PurchaseItemDto> Items) : IRequest<int>;
