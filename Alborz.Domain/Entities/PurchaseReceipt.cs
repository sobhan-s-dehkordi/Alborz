using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class PurchaseReceipt : BaseEntity
{
    private PurchaseReceipt() { }

    public PurchaseReceipt(string supplierName)
    {
        SupplierName = supplierName;
        ReceiptDate = DateTime.Now;
    }

    public string SupplierName { get; private set; }
    public DateTime ReceiptDate { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<PurchaseReceiptItem> _items = new();
    public IReadOnlyCollection<PurchaseReceiptItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity, decimal unitPrice)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

        var item = new PurchaseReceiptItem(product.Id, quantity, unitPrice);
        _items.Add(item);

        CalculateTotal();
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}