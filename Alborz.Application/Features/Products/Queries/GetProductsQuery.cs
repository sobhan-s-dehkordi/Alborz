using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public record GetProductsQuery(
        int? CodeFrom,
        int? CodeTo,
        string Barcode,
        string Name) : IRequest<List<ProductDto>>;
