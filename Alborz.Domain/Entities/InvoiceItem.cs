using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    private InvoiceItem() { }

    public InvoiceItem(int productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int InvoiceId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public Product? Product { get; private set; }
}
