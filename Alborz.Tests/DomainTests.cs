using Alborz.Domain.Entities;
using Alborz.Domain.Enums;
using Xunit;

namespace Alborz.Tests;

public sealed class DomainTests
{
    [Theory]
    [InlineData(0, 100, 0)]
    [InlineData(-1, 100, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(1, 100, -1)]
    [InlineData(1, 100, 101)]
    public void Rejects_invalid_invoice_lines(int quantity, decimal price, decimal discount) =>
        Assert.ThrowsAny<ArgumentException>(() => new InvoiceItem(1, quantity, price, discount));

    [Fact]
    public void Rejects_negative_charges_and_invalid_payment_method()
    {
        Assert.ThrowsAny<ArgumentException>(() => new Invoice(null, PaymentMethod.Cash, "", -1));
        Assert.ThrowsAny<ArgumentException>(() => new Invoice(null, (PaymentMethod)999));
    }

    [Fact]
    public void Rejects_invalid_initial_stock_and_empty_product_name()
    {
        Assert.ThrowsAny<ArgumentException>(() => new Product("P", "B", 1, 2, -1, 0));
        Assert.ThrowsAny<ArgumentException>(() => new Product(" ", "B", 1, 2, 0, 0));
    }

    [Fact]
    public void Rejects_excessive_receipt_discount()
    {
        var receipt = new PurchaseReceipt(1, DateTime.Today, "", 101, 0, "");
        receipt.AddItem(1, 1, 100, 0);
        Assert.Throws<ArgumentException>(() => receipt.ValidateTotals());
    }
}
