using Alborz.Application.Features.PurchaseReceipts.Commands;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public record GetPurchaseReceiptByIdQuery(int Id) : IRequest<PurchaseReceiptDto?>;