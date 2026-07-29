using MediatR;

namespace Alborz.Application.Features.Products.Commands;

public record CreateProductCommand(
        string Name,
        string Barcode,
        decimal PurchasePrice,
        decimal SellPrice,
        int InitialStock,
        int ReorderPoint) : IRequest<int>;
