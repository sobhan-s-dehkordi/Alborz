using MediatR;

namespace Alborz.Application.Features.Products.Commands;

public record UpdateProductCommand(
        int Id,
        string Name,
        string Barcode,
        decimal PurchasePrice,
        decimal SellPrice,
        int ReorderPoint) : IRequest<Unit>;
