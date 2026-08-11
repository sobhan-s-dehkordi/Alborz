using Alborz.Application.Contracts;
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

public class UpdatePurchaseReceiptCommandHandler : IRequestHandler<UpdatePurchaseReceiptCommand>
{
    private readonly IPurchaseReceiptRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseReceiptCommandHandler(IPurchaseReceiptRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        var receipt = await _repository.GetByIdWithItemsAsync(request.Id);

        if (receipt == null)
        {
            throw new Exception($"Purchase Receipt with ID {request.Id} not found.");
        }

        receipt.UpdateDetails(
            request.SupplierId,
            request.ReceiptDate,
            request.ReferenceNumber,
            request.TotalDiscount,
            request.AdditionalCharges,
            request.Remarks
        );

        receipt.ClearItems();

        foreach (var itemDto in request.Items)
        {
            receipt.AddItem(
                itemDto.ProductId,
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemDto.DiscountAmount
            );
        }

        _repository.Update(receipt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}