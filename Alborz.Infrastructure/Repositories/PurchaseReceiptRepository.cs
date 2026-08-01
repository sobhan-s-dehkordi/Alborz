using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;

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
}