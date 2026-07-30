using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Product : BaseEntity
{
    private Product() { }

    public Product(string name, string barcode, decimal purchasePrice, decimal sellPrice, int initialStock, int reorderPoint)
    {
        Name = name;
        Barcode = barcode;
        PurchasePrice = purchasePrice;
        SellPrice = sellPrice;
        StockQuantity = initialStock;
        ReorderPoint = reorderPoint;
    }

    public string Name { get; private set; }
    public string Barcode { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal SellPrice { get; private set; }
    public int StockQuantity { get; private set; }
    public int ReorderPoint { get; private set; }

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
