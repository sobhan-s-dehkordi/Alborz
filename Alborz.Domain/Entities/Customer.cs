using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string NationalCode { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public int LoyaltyPoints { get; private set; }

    private Customer() { }

    public Customer(string name, string phoneNumber, string nationalCode)
    {
        Name = Guard.Text(name, nameof(name), 150);
        PhoneNumber = Guard.Text(phoneNumber, nameof(phoneNumber), 20, false);
        NationalCode = Guard.Text(nationalCode, nameof(nationalCode), 20, false);
        Balance = 0;
        LoyaltyPoints = 0;
    }

    public void UpdateDetails(string name, string phoneNumber, string nationalCode)
    {
        Name = Guard.Text(name, nameof(name), 150);
        PhoneNumber = Guard.Text(phoneNumber, nameof(phoneNumber), 20, false);
        NationalCode = Guard.Text(nationalCode, nameof(nationalCode), 20, false);
    }

    public void AddLoyaltyPoints(decimal purchaseAmount)
    {
        var points = (int)(purchaseAmount / 100000);
        LoyaltyPoints += points;
    }

    public void DecreaseLoyaltyPoints(decimal invoiceAmount)
    {
        int pointsToRemove = (int)(invoiceAmount / 100000);

        LoyaltyPoints -= pointsToRemove;

        if (LoyaltyPoints < 0) LoyaltyPoints = 0;
    }

    public void UseLoyaltyPoints(int pointsToUse)
    {
        if (pointsToUse <= 0) throw new ArgumentOutOfRangeException(nameof(pointsToUse));
        if (LoyaltyPoints < pointsToUse)
            throw new InvalidOperationException("Insufficient loyalty points.");

        LoyaltyPoints -= pointsToUse;
    }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }
}


