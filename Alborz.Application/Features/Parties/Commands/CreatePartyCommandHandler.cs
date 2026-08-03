using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.Parties.Commands;

public class CreatePartyCommandHandler : IRequestHandler<CreatePartyCommand, int> {
    private readonly IPartyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePartyCommandHandler(IPartyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreatePartyCommand request, CancellationToken cancellationToken)
    {
        var party = new Party(request.Name, request.Phone, request.IsSupplier, request.IsCustomer);
        await _repository.AddAsync(party);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return party.Id;
    }
}
