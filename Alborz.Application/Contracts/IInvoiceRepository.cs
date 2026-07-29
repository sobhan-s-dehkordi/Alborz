using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Invoice invoice);
}
