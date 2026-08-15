using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    private InvoiceItem() { }

    public InvoiceItem(int productId, int quantity, decimal unitPrice, decimal discountAmount = 0)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountAmount = discountAmount;
    }

    public int InvoiceId { get; private set; }
    public int ProductId { get; private set; }

    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;

    public Product? Product { get; private set; }
}
