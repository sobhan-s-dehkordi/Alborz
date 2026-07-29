using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Product(string name, string barcode, decimal purchasePrice, decimal sellPrice, int initialStock, int reorderPoint) : BaseEntity
{
    public string Name { get; private set; } = name;
    public string Barcode { get; private set; } = barcode;
    public decimal PurchasePrice { get; private set; } = purchasePrice;
    public decimal SellPrice { get; private set; } = sellPrice;
    public int StockQuantity { get; private set; } = initialStock;
    public int ReorderPoint { get; private set; } = reorderPoint;

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Decrease quantity must be greater than zero.");

        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock for product {Name}.");

        StockQuantity -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Increase quantity must be greater than zero.");

        StockQuantity += quantity;
    }

    public bool NeedsReorder() => StockQuantity <= ReorderPoint;
}
