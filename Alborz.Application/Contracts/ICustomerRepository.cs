using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(int id);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
    Task AddAsync(Customer customer);
}
