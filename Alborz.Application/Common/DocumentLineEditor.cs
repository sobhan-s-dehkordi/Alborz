using Alborz.Domain.Common;

namespace Alborz.Application.Common;

public sealed record DocumentLine(int Quantity, decimal UnitPrice, decimal DiscountAmount);

public static class DocumentLineEditor
{
    // A repeated scan retains the existing row's price and discount rate.
    public static DocumentLine Increase(int productId, int quantity, decimal price, decimal discount, int addedQuantity)
    {
        Guard.Line(productId, quantity, price, discount);
        if (addedQuantity <= 0) throw new ArgumentException("Quantity must be positive.");
        var totalQuantity = checked(quantity + addedQuantity);
        var totalDiscount = decimal.Round(discount / quantity * totalQuantity, 2);
        return Validate(productId, totalQuantity, price, totalDiscount);
    }

    public static DocumentLine Validate(int productId, int quantity, decimal price, decimal discount)
    {
        Guard.Line(productId, quantity, price, discount);
        return new(quantity, price, discount);
    }
}
