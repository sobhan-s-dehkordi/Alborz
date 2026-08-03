using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public class GetPartiesQueryHandler : IRequestHandler<GetPartiesQuery, List<PartyDto>>
{
    private readonly IPartyRepository _repository;
    public GetPartiesQueryHandler(IPartyRepository repository) => _repository = repository;

    public async Task<List<PartyDto>> Handle(GetPartiesQuery request, CancellationToken cancellationToken)
    {
        var parties = await _repository.GetAllAsync();

        var query = parties.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(p => p.Name.Contains(request.Name, System.StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(p => p.Phone.Contains(request.Phone));

        return query.Select(p => new PartyDto(p.Id, p.Name, p.Phone, p.IsSupplier, p.IsCustomer)).ToList();
    }
}
