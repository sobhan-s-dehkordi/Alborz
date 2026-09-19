using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Product : BaseEntity
{
    private Product() { }

    public Product(string name, string barcode, decimal purchasePrice, decimal sellPrice, int initialStock, int reorderPoint)
    {
        name = Guard.Text(name, nameof(name), 200);
        barcode = Guard.Text(barcode, nameof(barcode), 50);
        Guard.Money(purchasePrice, nameof(purchasePrice));
        Guard.Money(sellPrice, nameof(sellPrice));
        if (reorderPoint < 0) throw new ArgumentOutOfRangeException(nameof(reorderPoint));
        Name = name;
        Barcode = barcode;
        PurchasePrice = purchasePrice;
        SellPrice = sellPrice;
        if (initialStock < 0) throw new ArgumentOutOfRangeException(nameof(initialStock));
        StockQuantity = initialStock;
        ReorderPoint = reorderPoint;
    }

    public string Name { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
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

        StockQuantity = checked(StockQuantity + quantity);
    }

    public void UpdateDetails(string name, string barcode, decimal purchasePrice, decimal sellPrice, int reorderPoint)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.");

        name = Guard.Text(name, nameof(name), 200);
        barcode = Guard.Text(barcode, nameof(barcode), 50);
        Guard.Money(purchasePrice, nameof(purchasePrice));
        Guard.Money(sellPrice, nameof(sellPrice));
        if (reorderPoint < 0) throw new ArgumentOutOfRangeException(nameof(reorderPoint));
        Name = name;
        Barcode = barcode;
        PurchasePrice = purchasePrice;
        SellPrice = sellPrice;
        ReorderPoint = reorderPoint;
    }

    public bool NeedsReorder() => StockQuantity <= ReorderPoint;
}


