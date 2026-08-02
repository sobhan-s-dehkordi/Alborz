using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public class CreatePurchaseReceiptCommandHandler : IRequestHandler<CreatePurchaseReceiptCommand, int>
{
    private readonly IPurchaseReceiptRepository _receiptRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseReceiptCommandHandler(
        IPurchaseReceiptRepository receiptRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _receiptRepository = receiptRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreatePurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        var receipt = new PurchaseReceipt(
            request.PartyId,
            request.ReceiptDate,
            request.ReferenceNumber,
            request.TotalDiscount,
            request.AdditionalCharges,
            request.Remarks);

        foreach (var itemDto in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            product.IncreaseStock(itemDto.Quantity);

            receipt.AddItem(product, itemDto.Quantity, itemDto.UnitPrice, itemDto.DiscountAmount);
        }

        await _receiptRepository.AddAsync(receipt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return receipt.Id;
    }
}
