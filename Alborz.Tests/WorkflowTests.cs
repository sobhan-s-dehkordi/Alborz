using Alborz.Application.Contracts;
using Alborz.Application.Features.Customers.Commands;
using Alborz.Application.Features.Invoices.Commands;
using Alborz.Application.Features.Invoices.Queries;
using Alborz.Application.Features.Products.Commands;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Application.Features.Reports.Queries;
using Alborz.Domain.Entities;
using Alborz.Domain.Enums;
using Alborz.Infrastructure.Data;
using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Alborz.Tests;

public sealed class WorkflowTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
{
    private async Task<T> Send<T>(IRequest<T> request)
    {
        using var scope = fixture.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
    }
    private async Task Send(IRequest request)
    {
        using var scope = fixture.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request);
    }
    private Task<int> Product(int stock = 20) => Send(new CreateProductCommand("Test product", Guid.NewGuid().ToString("N"), 50, 100, stock, 2));
    private Task<int> Customer() => Send(new CreateCustomerCommand("Test customer", "", ""));
    private async Task<int> Stock(int id)
    {
        using var scope = fixture.Services.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<AppDbContext>().Products.SingleAsync(p => p.Id == id)).StockQuantity;
    }
    private async Task<int> Points(int id)
    {
        using var scope = fixture.Services.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<AppDbContext>().Customers.SingleAsync(p => p.Id == id)).LoyaltyPoints;
    }
    private async Task<int> Supplier()
    {
        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var supplier = new Party("Supplier", "", true, false);
        db.Parties.Add(supplier);
        await db.SaveChangesAsync();
        return supplier.Id;
    }
    private static CreateInvoiceCommand Invoice(int product, int quantity = 2, int? customer = null,
        decimal price = 100, decimal lineDiscount = 10, decimal globalDiscount = 5) =>
        new(customer, PaymentMethod.Cash, globalDiscount, 3, "Test", [new(product, quantity, price, lineDiscount)]);

    [Fact]
    public async Task Sale_round_trips_line_discounts_and_updates_stock()
    {
        var product = await Product();
        var id = await Send(Invoice(product));
        var detail = await Send(new GetSalesInvoiceByIdQuery(id));
        Assert.NotNull(detail);
        Assert.Equal(10m, detail.Items.Single().DiscountAmount);
        Assert.Equal(18, await Stock(product));
    }

    [Fact]
    public async Task Editing_discounted_invoice_replaces_items_and_applies_stock_delta()
    {
        var product = await Product();
        var id = await Send(Invoice(product));
        await Send(new UpdateInvoiceCommand(id, null, PaymentMethod.CreditCard, 7, 2, "Updated", [new(product, 3, 100, 15)]));
        var detail = await Send(new GetSalesInvoiceByIdQuery(id));
        Assert.NotNull(detail);
        Assert.Single(detail.Items);
        Assert.Equal(15m, detail.Items[0].DiscountAmount);
        Assert.Equal(17, await Stock(product));
        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(1, await db.InvoiceItems.CountAsync(i => i.InvoiceId == id));
    }

    [Fact]
    public async Task Loyalty_uses_final_discounted_amount_and_corrects_rounding_on_edit()
    {
        var product = await Product();
        var customer = await Customer();
        var id = await Send(Invoice(product, 1, customer, 200000, 0, 50000));
        Assert.Equal(1, await Points(customer));
        await Send(new UpdateInvoiceCommand(id, customer, PaymentMethod.Cash, 0, 0, "", [new(product, 1, 200000, 0)]));
        Assert.Equal(2, await Points(customer));
    }

    [Fact]
    public async Task Changing_customer_moves_loyalty_credit()
    {
        var product = await Product();
        var first = await Customer();
        var second = await Customer();
        var id = await Send(Invoice(product, 1, first, 200000, 0, 0));
        await Send(new UpdateInvoiceCommand(id, second, PaymentMethod.Cash, 0, 0, "", [new(product, 1, 200000, 0)]));
        Assert.Equal(0, await Points(first));
        Assert.Equal(2, await Points(second));
    }

    [Fact]
    public async Task Invalid_sale_does_not_change_any_stock()
    {
        var first = await Product(10);
        var second = await Product(1);
        await Assert.ThrowsAsync<InvalidOperationException>(() => Send(new CreateInvoiceCommand(null, PaymentMethod.Cash, 0, 0, "",
            [new(first, 2, 100, 0), new(second, 2, 100, 0)])));
        Assert.Equal(10, await Stock(first));
        Assert.Equal(1, await Stock(second));
    }

    [Fact]
    public async Task Duplicate_lines_are_rejected_without_stock_changes()
    {
        var product = await Product();
        await Assert.ThrowsAsync<InvalidOperationException>(() => Send(new CreateInvoiceCommand(null, PaymentMethod.Cash, 0, 0, "",
            [new(product, 1, 100, 0), new(product, 1, 100, 0)])));
        Assert.Equal(20, await Stock(product));
    }

    [Fact]
    public async Task Purchase_and_edit_apply_only_stock_difference()
    {
        var product = await Product(0);
        var supplier = await Supplier();
        var id = await Send(new CreatePurchaseReceiptCommand(supplier, DateTime.Today, "A", 5, 2, "", [new(product, 5, 50, 10)]));
        Assert.Equal(5, await Stock(product));
        await Send(new UpdatePurchaseReceiptCommand(id, supplier, DateTime.Today, "B", 3, 0, "", [new(product, 3, 50, 5)]));
        Assert.Equal(3, await Stock(product));
        using var scope = fixture.Services.CreateScope();
        Assert.Equal(1, await scope.ServiceProvider.GetRequiredService<AppDbContext>().PurchaseReceiptItems.CountAsync(i => i.PurchaseReceiptId == id));
    }

    [Fact]
    public async Task Purchase_cannot_be_reduced_below_already_sold_stock()
    {
        var product = await Product(0);
        var supplier = await Supplier();
        var id = await Send(new CreatePurchaseReceiptCommand(supplier, DateTime.Today, "", 0, 0, "", [new(product, 5, 50, 0)]));
        await Send(Invoice(product, 4));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Send(new UpdatePurchaseReceiptCommand(id, supplier, DateTime.Today, "", 0, 0, "", [new(product, 2, 50, 0)])));
        Assert.Equal(1, await Stock(product));
    }

    [Fact]
    public async Task Reports_execute_in_sql_server_and_ledger_closes_at_current_stock()
    {
        var product = await Product(10);
        var customer = await Customer();
        await Send(Invoice(product, 2, customer));
        var ledger = await Send(new GetReportQuery(ReportKind.InventoryLedger, DateTime.Today, DateTime.Today, product));
        Assert.Equal(10, ledger[0].Balance);
        Assert.Equal(8, ledger[^1].Balance);
        var products = await Send(new GetReportQuery(ReportKind.TopProducts, DateTime.Today, DateTime.Today, null));
        Assert.Contains(products, r => r.Id == product && r.Amount == 190 && r.Quantity == 2);
        var customers = await Send(new GetReportQuery(ReportKind.TopCustomers, DateTime.Today, DateTime.Today, null));
        Assert.Contains(customers, r => r.Id == customer && r.Amount == 188 && r.Quantity == 1);
    }

    [Fact]
    public async Task Concurrent_stock_update_is_rejected()
    {
        var product = await Product(2);
        using var first = fixture.Services.CreateScope();
        using var second = fixture.Services.CreateScope();
        var firstDb = first.ServiceProvider.GetRequiredService<AppDbContext>();
        var secondDb = second.ServiceProvider.GetRequiredService<AppDbContext>();
        var a = await firstDb.Products.SingleAsync(p => p.Id == product);
        var b = await secondDb.Products.SingleAsync(p => p.Id == product);
        a.DecreaseStock(1);
        b.DecreaseStock(1);
        await firstDb.SaveChangesAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondDb.SaveChangesAsync());
        Assert.Equal(1, await Stock(product));
    }

    [Fact]
    public void Sales_export_contains_actual_numeric_values()
    {
        using var scope = fixture.Services.CreateScope();
        var bytes = scope.ServiceProvider.GetRequiredService<IExcelExportService>().ExportSalesInvoices(
            [new(42, DateTime.Today, "Customer", "Note", 200, 10, 2, 192)]);
        using var workbook = new XLWorkbook(new MemoryStream(bytes));
        var sheet = workbook.Worksheet(1);
        Assert.Equal(42, sheet.Cell(2, 1).GetValue<int>());
        Assert.Equal(192m, sheet.Cell(2, 8).GetValue<decimal>());
    }

    [Fact]
    public async Task Migrations_are_repeatable_and_snapshot_matches_model()
    {
        using var scope = fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        Assert.False(db.Database.HasPendingModelChanges());
    }
}
