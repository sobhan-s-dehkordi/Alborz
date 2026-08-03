using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IPartyRepository
{
    Task<Party?> GetByIdAsync(int id);
    Task<IEnumerable<Party>> GetAllAsync(string? searchTerm = null);
    Task AddAsync(Party party);
    void Update(Party party);
}