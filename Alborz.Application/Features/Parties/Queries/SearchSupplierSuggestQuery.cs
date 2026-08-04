using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public record SearchSupplierSuggestQuery(string SearchTerm) : IRequest<List<PartyDto>>;
