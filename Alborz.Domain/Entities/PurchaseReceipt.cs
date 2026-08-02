using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class PurchaseReceipt : BaseEntity
{
    private PurchaseReceipt() { }

    public PurchaseReceipt(int partyId, DateTime receiptDate, string referenceNumber, decimal totalDiscount, decimal additionalCharges, string remarks)
    {
        PartyId = partyId;
        ReceiptDate = receiptDate;
        ReferenceNumber = referenceNumber;
        TotalDiscount = totalDiscount;
        AdditionalCharges = additionalCharges;
        Remarks = remarks;
    }

    public int PartyId { get; private set; }
    public Party Party { get; private set; }

    public DateTime ReceiptDate { get; private set; }
    public string ReferenceNumber { get; private set; }

    public decimal TotalAmount { get; private set; }
    public decimal TotalDiscount { get; private set; }
    public decimal AdditionalCharges { get; private set; }
    public string Remarks { get; private set; }
    public decimal NetAmount => TotalAmount - TotalDiscount + AdditionalCharges;

    private readonly List<PurchaseReceiptItem> _items = new();
    public IReadOnlyCollection<PurchaseReceiptItem> Items => _items.AsReadOnly();

    public void AddItem(Product product, int quantity, decimal unitPrice, decimal discountAmount)
    {
        var item = new PurchaseReceiptItem(product.Id, quantity, unitPrice, discountAmount);
        _items.Add(item);
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}
