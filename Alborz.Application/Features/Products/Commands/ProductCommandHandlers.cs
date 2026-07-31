using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.Products.Commands;

public class ProductCommandHandlers :
        IRequestHandler<CreateProductCommand, int>,
        IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductCommandHandlers(IProductRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(request.Name, request.Barcode, request.PurchasePrice,
                                  request.SellPrice, request.InitialStock, request.ReorderPoint);

        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return product.Id;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) throw new KeyNotFoundException("Product not found.");

        product.UpdateDetails(request.Name, request.Barcode, request.PurchasePrice,
                              request.SellPrice, request.ReorderPoint);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}