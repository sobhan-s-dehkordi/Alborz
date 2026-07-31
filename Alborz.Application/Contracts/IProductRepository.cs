using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IProductRepository
{
    Task<IEnumerable<Product>> SearchAsync(int? codeFrom, int? codeTo, string barcode, string name);
    Task<Product> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
