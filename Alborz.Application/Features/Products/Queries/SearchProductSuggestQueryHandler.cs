using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public class SearchProductSuggestQueryHandler : IRequestHandler<SearchProductSuggestQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;

    public SearchProductSuggestQueryHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> Handle(SearchProductSuggestQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.SearchFastAsync(request.SearchTerm);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Barcode,
            p.PurchasePrice,
            p.SellPrice,
            p.StockQuantity
        )).ToList();
    }
}
