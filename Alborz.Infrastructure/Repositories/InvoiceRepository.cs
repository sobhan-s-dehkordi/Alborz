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

    public async Task<Invoice?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Invoice>> GetFilteredInvoicesAsync(int? customerId, DateTime? fromDate, DateTime? toDate, int? invoiceId)
    {
        var query = _context.Invoices
            .Include(i => i.Customer)
            .AsNoTracking()
            .AsQueryable();

        if (invoiceId.HasValue)
        {
            query = query.Where(i => i.Id == invoiceId.Value);
        }
        else
        {
            if (customerId.HasValue)
            {
                query = query.Where(i => i.CustomerId == customerId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.InvoiceDate <= endDate);
            }
        }

        return await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();
    }

    public void Update(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
    }
}