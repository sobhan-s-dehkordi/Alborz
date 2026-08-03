using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, List<PartyDto>>
{
    private readonly IPartyRepository _repository;

    public GetSuppliersQueryHandler(IPartyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PartyDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var allParties = await _repository.GetAllAsync();

        return allParties
            .Where(p => p.IsSupplier)
            .Select(p => new PartyDto(p.Id, p.Name, p.Phone, p.IsSupplier, p.IsCustomer))
            .ToList();
    }
}