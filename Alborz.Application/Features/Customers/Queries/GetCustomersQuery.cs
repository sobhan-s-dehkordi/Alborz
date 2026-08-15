using MediatR;

namespace Alborz.Application.Features.Customers.Queries;

public record GetCustomersQuery(string? Name, string? Phone, string? NationalCode)
    : IRequest<List<CustomerDto>>;
