using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Customers.Commands;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(ICustomerRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id);

        if (customer == null)
        {
            throw new Exception($"Customer with ID {request.Id} not found.");
        }

        customer.UpdateDetails(request.Name, request.PhoneNumber, request.NationalCode);

        _repository.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}