using Alborz.Application.Contracts;
using Alborz.Application.Features.Products.Queries;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public class GetProductByBarcodeQueryHandler : IRequestHandler<GetProductByBarcodeQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByBarcodeQueryHandler(IProductRepository repository) => _repository = repository;

    public async Task<ProductDto?> Handle(GetProductByBarcodeQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByBarcodeAsync(request.Barcode);
        if (product == null) return null;

        return new ProductDto(product.Id, product.Name, product.Barcode, product.PurchasePrice, product.SellPrice, product.StockQuantity);
    }
}