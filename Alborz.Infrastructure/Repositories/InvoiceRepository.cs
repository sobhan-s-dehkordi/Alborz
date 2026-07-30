using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Invoice invoice) =>
        await _context.Invoices.AddAsync(invoice);

    public async Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Invoices
            .Include(i => i.Items)
            .ThenInclude(item => item.Product)
            .Where(i => i.CustomerId == customerId)
            .ToListAsync();
    }
}