using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IPurchaseReceiptRepository
{
    Task AddAsync(PurchaseReceipt receipt);
    Task<IEnumerable<PurchaseReceipt>> GetReceiptsAsync(
        int? supplierId,
        DateTime? fromDate,
        DateTime? toDate,
        string referenceNumber);
}