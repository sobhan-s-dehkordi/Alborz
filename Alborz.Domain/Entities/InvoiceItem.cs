using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class InvoiceItem(int productId, int quantity, decimal unitPrice) : BaseEntity
{
    public int InvoiceId { get; private set; }
    public int ProductId { get; private set; } = productId;
    public int Quantity { get; private set; } = quantity;
    public decimal UnitPrice { get; private set; } = unitPrice;
    public decimal TotalPrice => Quantity * UnitPrice;

    public Product? Product { get; private set; }
}
