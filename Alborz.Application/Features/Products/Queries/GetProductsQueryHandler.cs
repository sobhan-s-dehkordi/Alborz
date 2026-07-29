using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchAsync(request.SearchTerm);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Barcode,
            p.SellPrice,
            p.StockQuantity
        )).ToList();
    }
}