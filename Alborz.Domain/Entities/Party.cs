using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Party : BaseEntity
{
    private Party() { }

    public Party(string name, string phone, bool isSupplier, bool isCustomer)
    {
        Name = Guard.Text(name, nameof(name), 150);
        Phone = Guard.Text(phone, nameof(phone), 20, false);
        if (!isSupplier && !isCustomer) throw new ArgumentException("Choose at least one party role.");
        IsSupplier = isSupplier;
        IsCustomer = isCustomer;
    }

    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public bool IsSupplier { get; private set; }
    public bool IsCustomer { get; private set; }

    public void Update(string name, string phone, bool isSupplier, bool isCustomer)
    {
        Name = Guard.Text(name, nameof(name), 150);
        Phone = Guard.Text(phone, nameof(phone), 20, false);
        if (!isSupplier && !isCustomer) throw new ArgumentException("Choose at least one party role.");
        IsSupplier = isSupplier;
        IsCustomer = isCustomer;
    }
}

