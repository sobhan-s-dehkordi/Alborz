using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public record SearchProductSuggestQuery(string SearchTerm) : IRequest<List<ProductDto>>;
