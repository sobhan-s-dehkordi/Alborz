using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
    Task<Product> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
    Task AddAsync(Product product);
}
