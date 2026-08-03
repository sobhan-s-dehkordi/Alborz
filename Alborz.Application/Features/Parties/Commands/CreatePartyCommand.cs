using MediatR;

namespace Alborz.Application.Features.Parties.Commands;

public record CreatePartyCommand(string Name, string Phone, bool IsSupplier, bool IsCustomer) : IRequest<int>;
