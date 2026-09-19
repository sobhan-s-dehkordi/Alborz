using Alborz.Application.Contracts;
using Alborz.Application.Features.Reports.Queries;
using Alborz.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alborz.Infrastructure.Repositories;

public sealed class ReportRepository(AppDbContext context) : IReportRepository
{
    public async Task<IReadOnlyList<ReportRow>> GetAsync(GetReportQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.From?.Date ?? DateTime.MinValue;
        var to = request.To?.Date.AddDays(1) ?? DateTime.MaxValue;
        if (request.Kind == ReportKind.InventoryLedger)
        {
            var product = await context.Products.AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException("Product not found.");
            var purchases = await (from receipt in context.PurchaseReceipts.AsNoTracking()
                                   from item in receipt.Items
                                   where item.ProductId == product.Id
                                   select new ReportRow(receipt.Id, product.Name, receipt.ReceiptDate, "Purchase",
                                       item.Quantity, item.Quantity * item.UnitPrice - item.DiscountAmount, null))
                                   .ToListAsync(cancellationToken);
            var sales = await (from invoice in context.Invoices.AsNoTracking()
                               from item in invoice.Items
                               where item.ProductId == product.Id
                               select new ReportRow(invoice.Id, product.Name, invoice.InvoiceDate, "Sale",
                                   -item.Quantity, item.Quantity * item.UnitPrice - item.DiscountAmount, null))
                               .ToListAsync(cancellationToken);
            var movements = purchases.Concat(sales).OrderBy(r => r.Date).ThenBy(r => r.Description).ThenBy(r => r.Id).ToList();
            var balance = checked(product.StockQuantity - movements.Sum(r => r.Quantity));
            foreach (var row in movements.Where(r => r.Date < startDate)) balance = checked(balance + row.Quantity);
            var result = new List<ReportRow> { new(product.Id, product.Name, request.From?.Date, "Opening balance", 0, 0, balance) };
            foreach (var row in movements.Where(r => r.Date >= startDate && r.Date < to))
            {
                balance = checked(balance + row.Quantity);
                result.Add(row with { Balance = balance });
            }
            return result;
        }
        if (request.Kind == ReportKind.TopProducts)
        {
            return await (from invoice in context.Invoices.AsNoTracking()
                          where invoice.InvoiceDate >= startDate && invoice.InvoiceDate < to
                          from item in invoice.Items
                          group item by new { item.ProductId, item.Product!.Name } into items
                          orderby items.Sum(i => i.Quantity * i.UnitPrice - i.DiscountAmount) descending, items.Key.ProductId
                          select new ReportRow(items.Key.ProductId, items.Key.Name, null, "Sales after line discounts",
                              items.Sum(i => i.Quantity), items.Sum(i => i.Quantity * i.UnitPrice - i.DiscountAmount), null))
                          .Take(100).ToListAsync(cancellationToken);
        }
        return await context.Invoices.AsNoTracking()
            .Where(i => i.CustomerId != null && i.InvoiceDate >= startDate && i.InvoiceDate < to)
            .GroupBy(i => new { i.CustomerId, i.Customer!.Name })
            .OrderByDescending(g => g.Sum(i => i.TotalAmount - i.DiscountAmount + i.AdditionalCharges))
            .ThenBy(g => g.Key.CustomerId)
            .Select(g => new ReportRow(g.Key.CustomerId!.Value, g.Key.Name, null, "Invoices / final sales amount",
                g.Count(), g.Sum(i => i.TotalAmount - i.DiscountAmount + i.AdditionalCharges), null))
            .Take(100).ToListAsync(cancellationToken);
    }
}

