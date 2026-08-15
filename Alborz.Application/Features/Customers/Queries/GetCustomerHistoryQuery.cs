using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Customers.Queries;

public record GetCustomerHistoryQuery(int CustomerId) : IRequest<List<CustomerHistoryDto>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repository;

    public GetCustomersQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.SearchAsync(request.Name, request.Phone, request.NationalCode);

        return customers.Select(c => new CustomerDto(
            c.Id,
            c.Name,
            c.PhoneNumber,
            c.NationalCode,
            c.Balance,
            c.LoyaltyPoints
        )).ToList();
    }
}
