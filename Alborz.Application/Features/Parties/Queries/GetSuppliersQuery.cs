using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public record GetSuppliersQuery() : IRequest<List<PartyDto>>;
