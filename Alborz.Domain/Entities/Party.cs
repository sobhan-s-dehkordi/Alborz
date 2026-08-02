using Alborz.Domain.Common;

namespace Alborz.Domain.Entities;

public class Party : BaseEntity
{
    private Party() { }

    public Party(string name, string phone, bool isSupplier, bool isCustomer)
    {
        Name = name;
        Phone = phone;
        IsSupplier = isSupplier;
        IsCustomer = isCustomer;
    }

    public string Name { get; private set; }
    public string Phone { get; private set; }
    public bool IsSupplier { get; private set; }
    public bool IsCustomer { get; private set; }
}