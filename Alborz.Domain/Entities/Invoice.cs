using Alborz.Domain.Common;
using Alborz.Domain.Enums;

namespace Alborz.Domain.Entities;

public class Invoice : BaseEntity
{
    private Invoice() { }

    public Invoice(int? customerId, PaymentMethod paymentMethod, string remarks = "", decimal additionalCharges = 0)
    {
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        Remarks = remarks ?? string.Empty;
        AdditionalCharges = additionalCharges;
        InvoiceDate = DateTime.Now;
        TotalAmount = 0;
        DiscountAmount = 0;
    }

    public int? CustomerId { get; private set; }
    public Customer? Customer { get; private set; }
    public DateTime InvoiceDate { get; private set; }

    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    public PaymentMethod PaymentMethod { get; private set; }

    public string Remarks { get; private set; }
    public decimal AdditionalCharges { get; private set; }

    public decimal TotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }

    public decimal FinalAmount => TotalAmount - DiscountAmount + AdditionalCharges;


    public void AddItem(int productId, int quantity, decimal unitPrice, decimal discountAmount = 0)
    {
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            throw new InvalidOperationException("This product has already been added to the invoice.");
        }

        var item = new InvoiceItem(productId, quantity, unitPrice, discountAmount);
        _items.Add(item);

        CalculateTotal();
    }

    public void ApplyGlobalDiscount(decimal discount)
    {
        if (discount < 0) throw new ArgumentException("Discount cannot be negative.");
        if (discount > TotalAmount) throw new ArgumentException("Discount cannot be greater than the total amount.");

        DiscountAmount = discount;
    }

    public void UpdateAdditionalCharges(decimal charges)
    {
        if (charges < 0) throw new ArgumentException("Charges cannot be negative.");
        AdditionalCharges = charges;
    }

    public void UpdateRemarks(string remarks)
    {
        Remarks = remarks ?? string.Empty;
    }

    public void UpdateHeader(int? customerId, PaymentMethod paymentMethod, string remarks, decimal globalDiscount, decimal additionalCharges)
    {
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        UpdateRemarks(remarks);
        UpdateAdditionalCharges(additionalCharges);
        ApplyGlobalDiscount(globalDiscount);
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    public void ClearItems()
    {
        _items.Clear();
        CalculateTotal();
    }
}
