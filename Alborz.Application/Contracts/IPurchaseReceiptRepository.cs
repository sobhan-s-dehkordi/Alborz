using Alborz.Domain.Entities;

namespace Alborz.Application.Contracts;

public interface IPurchaseReceiptRepository
{
    Task AddAsync(PurchaseReceipt receipt);
}