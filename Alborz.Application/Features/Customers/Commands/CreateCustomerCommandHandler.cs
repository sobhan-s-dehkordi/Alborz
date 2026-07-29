using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.Customers.Commands;

public class CreateCustomerCommandHandler(ICustomerRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, int>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer(request.Name, request.PhoneNumber, request.NationalCode);

        await _repository.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
