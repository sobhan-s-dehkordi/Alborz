using Alborz.Application.Features.PurchaseReceipts.Commands;

namespace Alborz.Application.Contracts;

public interface IExcelExportService
{
    byte[] ExportPurchaseReceipts(IEnumerable<PurchaseReceiptDto> receipts);
}