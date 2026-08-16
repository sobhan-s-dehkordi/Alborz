using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Commands;

public class UpdatePurchaseReceiptCommandHandler : IRequestHandler<UpdatePurchaseReceiptCommand>
{
    private readonly IPurchaseReceiptRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseReceiptCommandHandler(
        IPurchaseReceiptRepository repository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePurchaseReceiptCommand request, CancellationToken cancellationToken)
    {
        if (request.Items == null || !request.Items.Any())
            throw new ArgumentException("The purchase receipt must contain at least one item.");

        var receipt = await _repository.GetByIdWithItemsAsync(request.Id);
        if (receipt == null)
            throw new KeyNotFoundException($"Purchase Receipt with ID {request.Id} not found.");


        var oldQuantities = receipt.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var newQuantities = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var allProductIds = oldQuantities.Keys.Union(newQuantities.Keys).Distinct();

        foreach (var productId in allProductIds)
        {
            int oldQty = oldQuantities.TryGetValue(productId, out var o) ? o : 0;
            int newQty = newQuantities.TryGetValue(productId, out var n) ? n : 0;

            int diff = newQty - oldQty;

            if (diff == 0) continue;

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product ID {productId} not found.");

            if (diff > 0)
            {
                product.IncreaseStock(diff);
            }
            else
            {
                int amountToDecrease = Math.Abs(diff);

                if (product.StockQuantity < amountToDecrease)
                {
                    throw new InvalidOperationException(
                        $"Cannot reduce quantity for product '{product.Name}'. " +
                        $"Current stock is {product.StockQuantity}, but you are trying to reduce it by {amountToDecrease}. " +
                        $"This usually happens if the product has already been sold.");
                }

                product.DecreaseStock(amountToDecrease);
            }
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
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            receipt.AddItem(product, itemDto.Quantity, itemDto.UnitPrice, itemDto.DiscountAmount);
        }

        _repository.Update(receipt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}