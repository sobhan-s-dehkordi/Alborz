using MediatR;

namespace Alborz.Application.Features.Parties.Queries;

public record GetPartiesQuery(string? Name, string? Phone) : IRequest<List<PartyDto>>;
