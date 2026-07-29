using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Customer(string name, string phoneNumber, string nationalCode) : BaseEntity
{
    public string Name { get; private set; } = name;
    public string PhoneNumber { get; private set; } = phoneNumber;
    public string NationalCode { get; private set; } = nationalCode;
    public decimal Balance { get; private set; } = 0;
    public int LoyaltyPoints { get; private set; } = 0;

    public void AddLoyaltyPoints(decimal purchaseAmount)
    {
        var points = (int)(purchaseAmount / 100000);
        LoyaltyPoints += points;
    }

    public void UseLoyaltyPoints(int pointsToUse)
    {
        if (LoyaltyPoints < pointsToUse)
            throw new InvalidOperationException("Insufficient loyalty points.");

        LoyaltyPoints -= pointsToUse;
    }

    public void UpdateBalance(decimal amount)
    {
        Balance += amount;
    }
}
