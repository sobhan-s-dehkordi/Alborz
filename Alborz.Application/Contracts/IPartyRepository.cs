using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IPartyRepository
{
    Task<Party?> GetByIdAsync(int id);
    Task<List<Party>> GetSuppliersAsync();
    Task<List<Party>> GetCustomersAsync();
    Task AddAsync(Party party);
}