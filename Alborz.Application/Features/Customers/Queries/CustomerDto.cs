namespace Alborz.Application.Features.Customers.Queries;

public record CustomerDto(
    int Id,
    string Name,
    string PhoneNumber,
    string NationalCode,
    decimal Balance,
    int LoyaltyPoints
);
