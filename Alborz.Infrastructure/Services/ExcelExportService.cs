using Alborz.Application.Contracts;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using ClosedXML.Excel;

namespace Alborz.Infrastructure.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] ExportPurchaseReceipts(IEnumerable<PurchaseReceiptDto> receipts)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Purchase Receipts");

        worksheet.Cell(1, 1).Value = "ID";
        worksheet.Cell(1, 2).Value = "Date";
        worksheet.Cell(1, 3).Value = "Reference Number";
        worksheet.Cell(1, 4).Value = "Supplier Name";
        worksheet.Cell(1, 5).Value = "Total Amount";
        worksheet.Cell(1, 6).Value = "Net Amount";

        var headerRow = worksheet.Range("A1:F1");
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Medium;

        int row = 2;
        foreach (var receipt in receipts)
        {
            worksheet.Cell(row, 1).Value = receipt.Id;
            worksheet.Cell(row, 2).Value = receipt.ReceiptDate.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 3).Value = receipt.ReferenceNumber;
            worksheet.Cell(row, 4).Value = receipt.SupplierName;

            worksheet.Cell(row, 5).Value = receipt.TotalAmount;
            worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";

            worksheet.Cell(row, 6).Value = receipt.NetAmount;
            worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}