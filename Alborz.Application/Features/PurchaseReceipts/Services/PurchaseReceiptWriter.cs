using Alborz.Application.Contracts;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Domain.Entities;

namespace Alborz.Application.Features.PurchaseReceipts.Services;

public sealed class PurchaseReceiptWriter(
    IPurchaseReceiptRepository receipts, IProductRepository products,
    IPartyRepository parties, IUnitOfWork unitOfWork)
{
    public async Task<int> SaveAsync(int? id, int supplierId, DateTime date, string reference,
        decimal discount, decimal charges, string remarks, List<PurchaseItemDto> items,
        CancellationToken cancellationToken)
    {
        if (items is null || items.Count == 0)
            throw new ArgumentException("The purchase receipt must contain at least one item.");
        var supplier = await parties.GetByIdAsync(supplierId);
        if (supplier is null || !supplier.IsSupplier)
            throw new ArgumentException("Select a valid supplier.");
        var draft = new PurchaseReceipt(supplierId, date, reference, discount, charges, remarks);
        foreach (var item in items)
            draft.AddItem(item.ProductId, item.Quantity, item.UnitPrice, item.DiscountAmount);
        draft.ValidateTotals();
        var receipt = id.HasValue
            ? await receipts.GetByIdWithItemsAsync(id.Value)
                ?? throw new KeyNotFoundException("Purchase receipt not found.")
            : draft;
        var oldQuantities = id.HasValue
            ? receipt.Items.ToDictionary(i => i.ProductId, i => i.Quantity)
            : new Dictionary<int, int>();
        var newQuantities = items.ToDictionary(i => i.ProductId, i => i.Quantity);
        var changes = new List<(Product Product, int Delta)>();
        foreach (var productId in oldQuantities.Keys.Union(newQuantities.Keys).Order())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var product = await products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");
            var delta = newQuantities.GetValueOrDefault(productId) - oldQuantities.GetValueOrDefault(productId);
            if ((long)product.StockQuantity + delta < 0 || (long)product.StockQuantity + delta > int.MaxValue)
                throw new InvalidOperationException($"The receipt would make stock invalid for '{product.Name}'.");
            changes.Add((product, delta));
        }
        foreach (var (product, delta) in changes)
        {
            if (delta > 0) product.IncreaseStock(delta);
            else if (delta < 0) product.DecreaseStock(-delta);
        }
        if (id.HasValue)
        {
            receipt.ClearItems();
            foreach (var item in items)
                receipt.AddItem(item.ProductId, item.Quantity, item.UnitPrice, item.DiscountAmount);
            receipt.UpdateDetails(supplierId, date, reference, discount, charges, remarks);
        }
        if (!id.HasValue) await receipts.AddAsync(receipt);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return receipt.Id;
    }
}
