using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<Customer> GetByIdAsync(int id) =>
        await _context.Customers.FindAsync(id);

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _context.Customers.ToListAsync();

        return await _context.Customers
            .Where(c => c.Name.Contains(searchTerm) || c.PhoneNumber.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task AddAsync(Customer customer) =>
        await _context.Customers.AddAsync(customer);
}

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

    public async Task<List<Party>> GetSuppliersAsync()
    {
        return await _context.Parties
                             .Where(x => x.IsSupplier)
                             .OrderBy(x => x.Name)
                             .ToListAsync();
    }

    public async Task<List<Party>> GetCustomersAsync()
    {
        return await _context.Parties
                             .Where(x => x.IsCustomer)
                             .OrderBy(x => x.Name)
                             .ToListAsync();
    }

    public async Task AddAsync(Party party)
    {
        await _context.Parties.AddAsync(party);
    }
}