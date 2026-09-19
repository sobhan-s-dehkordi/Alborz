using Alborz.Application.Features.PurchaseReceipts.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;

namespace Alborz.Application.Contracts;

public interface IExcelExportService
{
    byte[] ExportSalesInvoices(IEnumerable<Alborz.Application.Features.Invoices.Queries.SalesInvoiceDto> invoices);
    byte[] ExportPurchaseReceipts(IEnumerable<PurchaseReceiptDto> receipts);
}

