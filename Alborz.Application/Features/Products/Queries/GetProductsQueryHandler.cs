using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.SearchAsync(
            request.CodeFrom,
            request.CodeTo,
            request.Barcode,
            request.Name
        );

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Barcode,
            p.PurchasePrice,
            p.SellPrice,
            p.StockQuantity
        )).ToList();

        return productDtos;
    }
}