using Alborz.Application.Features.Products.Queries;
using MediatR;

namespace Alborz.Application.Features.PurchaseReceipts.Queries;

public record GetProductByBarcodeQuery(string Barcode) : IRequest<ProductDto?>;
