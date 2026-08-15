using MediatR;

namespace Alborz.Application.Features.Customers.Commands;

public record UpdateCustomerCommand(
    int Id,
    string Name,
    string PhoneNumber,
    string NationalCode
    ) : IRequest;
