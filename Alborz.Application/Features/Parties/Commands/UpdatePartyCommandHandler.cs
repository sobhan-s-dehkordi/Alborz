using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Parties.Commands;

public class UpdatePartyCommandHandler : IRequestHandler<UpdatePartyCommand>
{
    private readonly IPartyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePartyCommandHandler(IPartyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePartyCommand request, CancellationToken cancellationToken)
    {
        var party = await _repository.GetByIdAsync(request.Id);
        if (party != null)
        {
            party.Update(request.Name, request.Phone, request.IsSupplier, request.IsCustomer);
            _repository.Update(party);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}