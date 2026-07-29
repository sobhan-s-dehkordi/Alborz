using MediatR;

namespace Alborz.Application.Features.Customers.Commands;

public record CreateCustomerCommand(string Name, string PhoneNumber, string NationalCode) : IRequest<int>;