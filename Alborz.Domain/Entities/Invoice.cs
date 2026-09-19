using Alborz.Domain.Common;
using Alborz.Domain.Enums;

namespace Alborz.Domain.Entities;

public class Invoice : BaseEntity
{
    private Invoice() { }

    public Invoice(int? customerId, PaymentMethod paymentMethod, string remarks = "", decimal additionalCharges = 0)
    {
        if (!Enum.IsDefined(paymentMethod)) throw new ArgumentException("Invalid payment method.");
        if (customerId <= 0) throw new ArgumentException("Invalid customer.");
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        Remarks = Guard.Text(remarks, nameof(remarks), 1000, false);
        UpdateAdditionalCharges(additionalCharges);
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

    public string Remarks { get; private set; } = string.Empty;
    public decimal AdditionalCharges { get; private set; }

    public decimal TotalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }

    public decimal FinalAmount => TotalAmount - DiscountAmount + AdditionalCharges;


    public void ChangeDate(DateTime date)
    {
        if (date == default) throw new ArgumentException("Invoice date is required.");
        InvoiceDate = date;
    }

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
        Guard.Money(discount, nameof(discount));
        if (discount < 0) throw new ArgumentException("Discount cannot be negative.");
        if (discount > TotalAmount) throw new ArgumentException("Discount cannot be greater than the total amount.");

        DiscountAmount = discount;
    }

    public void UpdateAdditionalCharges(decimal charges)
    {
        Guard.Money(charges, nameof(charges));
        if (charges < 0) throw new ArgumentException("Charges cannot be negative.");
        AdditionalCharges = charges;
    }

    public void UpdateRemarks(string remarks)
    {
        Remarks = Guard.Text(remarks, nameof(remarks), 1000, false);
    }

    public void UpdateHeader(int? customerId, PaymentMethod paymentMethod, string remarks, decimal globalDiscount, decimal additionalCharges)
    {
        if (!Enum.IsDefined(paymentMethod)) throw new ArgumentException("Invalid payment method.");
        if (customerId <= 0) throw new ArgumentException("Invalid customer.");
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        UpdateRemarks(remarks);
        UpdateAdditionalCharges(additionalCharges);
        ApplyGlobalDiscount(globalDiscount);
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
        Guard.Money(TotalAmount + AdditionalCharges, nameof(TotalAmount));
    }

    public void ClearItems()
    {
        _items.Clear();
        CalculateTotal();
    }
}



