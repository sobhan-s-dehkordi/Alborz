using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class PartyRepository : IPartyRepository
{
    private readonly AppDbContext _context;

    public PartyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Party?> GetByIdAsync(int id)
    {
        return await _context.Parties.FindAsync(id);
    }

    public async Task<IEnumerable<Party>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Parties.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Phone.Contains(searchTerm));
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task AddAsync(Party party)
    {
        await _context.Parties.AddAsync(party);
    }

    public void Update(Party party)
    {
        _context.Parties.Update(party);
    }
}