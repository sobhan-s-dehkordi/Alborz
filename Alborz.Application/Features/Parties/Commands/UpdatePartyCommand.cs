using MediatR;

namespace Alborz.Application.Features.Parties.Commands;

public record UpdatePartyCommand(int Id, string Name, string Phone, bool IsSupplier, bool IsCustomer) : IRequest;
