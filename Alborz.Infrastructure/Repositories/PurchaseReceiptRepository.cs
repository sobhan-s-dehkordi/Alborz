using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class PurchaseReceiptRepository : IPurchaseReceiptRepository
{
    private readonly AppDbContext _context;

    public PurchaseReceiptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PurchaseReceipt receipt)
    {
        await _context.PurchaseReceipts.AddAsync(receipt);
    }

    public async Task<IEnumerable<PurchaseReceipt>> GetReceiptsAsync(
            int? supplierId,
            DateTime? fromDate,
            DateTime? toDate,
            string referenceNumber)
    {
        var query = _context.PurchaseReceipts
            .AsNoTracking()
            .Include(pr => pr.Party)
            .Include(pr => pr.Items)
                .ThenInclude(i => i.Product)
            .AsQueryable();

        if (supplierId.HasValue)
        {
            query = query.Where(q => q.PartyId == supplierId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(q => q.ReceiptDate.Date >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(q => q.ReceiptDate.Date <= toDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(referenceNumber))
        {
            query = query.Where(q => q.ReferenceNumber.Contains(referenceNumber));
        }

        return await query
            .OrderByDescending(q => q.ReceiptDate)
            .ToListAsync();
    }
    public async Task<PurchaseReceipt?> GetByIdWithItemsAsync(int id)
    {
        return await _context.PurchaseReceipts
            .Include(pr => pr.Party)
            .Include(pr => pr.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public void Update(PurchaseReceipt receipt)
    {
        _context.PurchaseReceipts.Update(receipt);
    }
}