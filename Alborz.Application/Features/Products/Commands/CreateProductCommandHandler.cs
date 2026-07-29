using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.Products.Commands;

public class CreateProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await _repository.GetByBarcodeAsync(request.Barcode);
        if (existingProduct != null)
            throw new InvalidOperationException($"A product with barcode '{request.Barcode}' already exists.");

        var product = new Product(
            request.Name,
            request.Barcode,
            request.PurchasePrice,
            request.SellPrice,
            request.InitialStock,
            request.ReorderPoint);

        await _repository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}