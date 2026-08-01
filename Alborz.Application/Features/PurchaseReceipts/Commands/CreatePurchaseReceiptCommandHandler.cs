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
        if (!request.Items.Any()) throw new ArgumentException("Receipt must contain at least one item.");

        var receipt = new PurchaseReceipt(request.SupplierName);

        foreach (var itemDto in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null) throw new KeyNotFoundException($"Product ID {itemDto.ProductId} not found.");

            // Domain Logic: Increase the stock!
            product.IncreaseStock(itemDto.Quantity);

            // Add to receipt
            receipt.AddItem(product, itemDto.Quantity, itemDto.UnitPrice);
        }

        await _receiptRepository.AddAsync(receipt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return receipt.Id;
    }
}