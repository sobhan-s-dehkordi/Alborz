using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Alborz.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<Customer> GetByIdAsync(int id) =>
        await _context.Customers.FindAsync(id);

    public async Task<List<Customer>> SearchAsync(string? name, string? phone, string? nationalCode)
    {
        var query = _context.Customers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            query = query.Where(c => c.PhoneNumber.Contains(phone));
        }

        if (!string.IsNullOrWhiteSpace(nationalCode))
        {
            query = query.Where(c => c.NationalCode.Contains(nationalCode));
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Customer customer) =>
        await _context.Customers.AddAsync(customer);

    public void Update(Customer customer) =>
        _context.Customers.Update(customer);
    
}
