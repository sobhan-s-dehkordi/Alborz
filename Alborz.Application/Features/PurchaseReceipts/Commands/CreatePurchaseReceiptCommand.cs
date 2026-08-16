using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public record CreatePurchaseReceiptCommand(
    int PartyId,
    DateTime ReceiptDate,
    string ReferenceNumber,
    decimal TotalDiscount,
    decimal AdditionalCharges,
    string Remarks,
    List<PurchaseItemDto> Items) : IRequest<int>;
