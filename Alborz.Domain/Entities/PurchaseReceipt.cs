using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class PurchaseReceipt : BaseEntity
{
    private PurchaseReceipt() { }

    public PurchaseReceipt(int partyId, DateTime receiptDate, string referenceNumber, decimal totalDiscount, decimal additionalCharges, string remarks)
    {
        if (partyId <= 0) throw new ArgumentOutOfRangeException(nameof(partyId));
        Guard.Money(totalDiscount, nameof(totalDiscount));
        Guard.Money(additionalCharges, nameof(additionalCharges));
        PartyId = partyId;
        ReceiptDate = receiptDate;
        ReferenceNumber = Guard.Text(referenceNumber, nameof(referenceNumber), 50, false);
        TotalDiscount = totalDiscount;
        AdditionalCharges = additionalCharges;
        Remarks = Guard.Text(remarks, nameof(remarks), 1000, false);
    }

    public int PartyId { get; private set; }
    public Party Party { get; private set; } = null!;

    public DateTime ReceiptDate { get; private set; }
    public string ReferenceNumber { get; private set; } = string.Empty;

    public decimal TotalAmount { get; private set; }
    public decimal TotalDiscount { get; private set; }
    public decimal AdditionalCharges { get; private set; }
    public string Remarks { get; private set; } = string.Empty;
    public decimal NetAmount => TotalAmount - TotalDiscount + AdditionalCharges;

    private readonly List<PurchaseReceiptItem> _items = new();
    public IReadOnlyCollection<PurchaseReceiptItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity, decimal unitPrice, decimal discountAmount)
    {
        AddItem(product.Id, quantity, unitPrice, discountAmount);
    }

    public void ValidateTotals()
    {
        Guard.Money(TotalAmount + AdditionalCharges, nameof(TotalAmount));
        if (TotalDiscount > TotalAmount)
            throw new ArgumentException("Receipt discount exceeds the total amount.");
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }

    public void UpdateDetails(int partyId, DateTime receiptDate, string referenceNumber, decimal totalDiscount, decimal additionalCharges, string remarks)
    {
        if (partyId <= 0) throw new ArgumentOutOfRangeException(nameof(partyId));
        Guard.Money(totalDiscount, nameof(totalDiscount));
        Guard.Money(additionalCharges, nameof(additionalCharges));
        PartyId = partyId;
        ReceiptDate = receiptDate;
        ReferenceNumber = Guard.Text(referenceNumber, nameof(referenceNumber), 50, false);
        TotalDiscount = totalDiscount;
        AdditionalCharges = additionalCharges;
        Remarks = Guard.Text(remarks, nameof(remarks), 1000, false);

        CalculateTotal();
    }

    public void ClearItems()
    {
        _items.Clear();
        CalculateTotal();
    }

    public void AddItem(int productId, int quantity, decimal unitPrice, decimal discountAmount)
    {
        if (_items.Any(i => i.ProductId == productId)) throw new ArgumentException("Duplicate product in receipt.");
        var item = new PurchaseReceiptItem(productId, quantity, unitPrice, discountAmount);
        _items.Add(item);
        CalculateTotal();
    }
}


