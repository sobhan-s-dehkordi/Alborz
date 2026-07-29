using Alborz.Domain.Common;
using Alborz.Domain.Enums;

namespace Alborz.Domain.Entities;

public class Invoice(int? customerId, PaymentMethod paymentMethod) : BaseEntity
{
    public int? CustomerId { get; private set; } = customerId;
    public Customer? Customer { get; private set; }
    public DateTime InvoiceDate { get; private set; } = DateTime.Now;

    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public decimal TotalAmount { get; private set; } = 0;
    public decimal DiscountAmount { get; private set; } = 0;
    public decimal FinalAmount => TotalAmount - DiscountAmount;

    public PaymentMethod PaymentMethod { get; private set; } = paymentMethod;

    public void AddItem(Product product, int quantity)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == product.Id);
        if (existingItem != null)
        {
            throw new InvalidOperationException("This product has already been added to the invoice.");
        }

        var item = new InvoiceItem(product.Id, quantity, product.SellPrice);
        _items.Add(item);

        CalculateTotal();
    }

    public void ApplyDiscount(decimal discount)
    {
        if (discount < 0 || discount > TotalAmount)
            throw new ArgumentException("Invalid discount amount.");

        DiscountAmount = discount;
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}
