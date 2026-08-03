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
