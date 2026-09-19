namespace Alborz.Domain.Common;

public static class Guard
{
    public static string Text(string? value, string name, int maxLength, bool required = true)
    {
        value = value?.Trim() ?? string.Empty;
        if ((required && value.Length == 0) || value.Length > maxLength)
            throw new ArgumentException($"{name} must contain {(required ? 1 : 0)} to {maxLength} characters.", name);
        return value;
    }

    public static void Money(decimal value, string name)
    {
        if (value < 0 || value > 9999999999999999.99m || decimal.Round(value, 2) != value)
            throw new ArgumentOutOfRangeException(name, "Amount must be non-negative with at most two decimal places.");
    }

    public static void Line(int productId, int quantity, decimal price, decimal discount)
    {
        if (productId <= 0 || quantity <= 0)
            throw new ArgumentException("Product and quantity must be positive.");
        Money(price, nameof(price));
        Money(discount, nameof(discount));
        Money(quantity * price, "Line total");
        if (discount > quantity * price)
            throw new ArgumentException("Line discount exceeds the line amount.");
    }
}
