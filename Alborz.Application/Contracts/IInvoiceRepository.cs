using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IInvoiceRepository
{
    Task<IEnumerable<Invoice>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Invoice invoice);
    void Update(Invoice invoice);
    Task<Invoice?> GetByIdWithDetailsAsync(int id);
    Task<List<Invoice>> GetFilteredInvoicesAsync(int? customerId, DateTime? fromDate, DateTime? toDate, int? invoiceId);
}
