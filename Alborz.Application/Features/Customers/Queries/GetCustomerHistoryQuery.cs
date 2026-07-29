using MediatR;

namespace Alborz.Application.Features.Customers.Queries;

public record GetCustomerHistoryQuery(int CustomerId) : IRequest<List<CustomerHistoryDto>>;
