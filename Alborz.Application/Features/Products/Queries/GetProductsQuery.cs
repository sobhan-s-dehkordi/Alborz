using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public record GetProductsQuery(string SearchTerm) : IRequest<List<ProductDto>>;
