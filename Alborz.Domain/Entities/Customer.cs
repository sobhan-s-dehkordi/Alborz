using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; }
    public string PhoneNumber { get; private set; }
    public string NationalCode { get; private set; }
    public decimal Balance { get; private set; }
    public int LoyaltyPoints { get; private set; }

    private Customer() { }

    public Customer(string name, string phoneNumber, string nationalCode)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        NationalCode = nationalCode;
        Balance = 0;
        LoyaltyPoints = 0;
    }

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
