using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<Product> GetByIdAsync(int id) =>
        await _context.Products.FindAsync(id);

    public async Task<Product> GetByBarcodeAsync(string barcode) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _context.Products.ToListAsync();

        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Barcode == searchTerm)
            .ToListAsync();
    }

    public async Task AddAsync(Product product) =>
        await _context.Products.AddAsync(product);
}
