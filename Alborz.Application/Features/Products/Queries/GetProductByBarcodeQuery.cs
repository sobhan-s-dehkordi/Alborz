using MediatR;

namespace Alborz.Application.Features.Products.Queries;

public record GetProductByBarcodeQuery(string Barcode) : IRequest<ProductDto?>;
