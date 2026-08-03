namespace Alborz.Application.Features.Parties.Queries;

public record PartyDto(int Id, string Name, string Phone, bool IsSupplier, bool IsCustomer)
{
    public string Role => (IsSupplier && IsCustomer) ? "Supplier & Customer" : IsSupplier ? "Supplier" : IsCustomer ? "Customer" : "None";
}
