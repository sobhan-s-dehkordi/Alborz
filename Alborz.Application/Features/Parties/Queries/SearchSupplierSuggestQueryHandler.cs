using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public class SearchSupplierSuggestQueryHandler : IRequestHandler<SearchSupplierSuggestQuery, List<PartyDto>>
{
    private readonly IPartyRepository _repository;

    public SearchSupplierSuggestQueryHandler(IPartyRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PartyDto>> Handle(SearchSupplierSuggestQuery request, CancellationToken cancellationToken)
    {
        var parties = await _repository.SearchSuppliersFastAsync(request.SearchTerm);

        return parties.Select(p => new PartyDto(
            p.Id,
            p.Name,
            p.Phone,
            p.IsSupplier,
            p.IsCustomer
        )).ToList();
    }
}
