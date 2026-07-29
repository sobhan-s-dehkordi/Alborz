namespace Alborz.Application.Features.Products.Queries;

public record ProductDto(int Id, string Name, string Barcode, decimal SellPrice, int StockQuantity);
