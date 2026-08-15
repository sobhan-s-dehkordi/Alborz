using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(int id);
    Task<List<Customer>> SearchAsync(string? name, string? phone, string? nationalCode);
    Task AddAsync(Customer customer);
    void Update(Customer customer);
}
