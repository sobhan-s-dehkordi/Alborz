using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> SearchAsync(int? codeFrom, int? codeTo, string barcode, string name)
    {
        var query = _context.Products.AsQueryable();

        if (codeFrom.HasValue)
            query = query.Where(p => p.Id >= codeFrom.Value);

        if (codeTo.HasValue)
            query = query.Where(p => p.Id <= codeTo.Value);

        if (!string.IsNullOrWhiteSpace(barcode))
            query = query.Where(p => p.Barcode.Contains(barcode));

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        return await query.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

    public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return null;

        return await _context.Products
            .FirstOrDefaultAsync(p => p.Barcode == barcode);
    }
}
