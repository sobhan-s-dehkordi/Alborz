using Alborz.Application.Contracts;
using Alborz.Infrastructure.Data;

namespace Alborz.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("This record was changed by another user. Reload it and try again.", ex);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            throw new InvalidOperationException("Changes could not be saved. Check duplicate barcodes and referenced records, then retry.", ex);
        }
    }
}

