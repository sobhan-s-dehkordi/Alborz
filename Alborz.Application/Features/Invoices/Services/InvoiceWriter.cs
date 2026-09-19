using Alborz.Application.Contracts;
using Alborz.Application.Features.Invoices.Commands;
using Alborz.Domain.Entities;
using Alborz.Domain.Enums;

namespace Alborz.Application.Features.Invoices.Services;

public sealed class InvoiceWriter(
    IInvoiceRepository invoices, IProductRepository products,
    ICustomerRepository customers, IUnitOfWork unitOfWork)
{
    public async Task<int> SaveAsync(int? id, int? customerId, PaymentMethod paymentMethod,
        decimal discount, decimal charges, string remarks, List<InvoiceItemDto> items,
        CancellationToken cancellationToken, DateTime? invoiceDate = null)
    {
        if (items is null || items.Count == 0)
            throw new ArgumentException("The invoice must contain at least one item.");

        var draft = new Invoice(customerId, paymentMethod, remarks, charges);
        foreach (var item in items)
            draft.AddItem(item.ProductId, item.Quantity, item.UnitPrice, item.DiscountAmount);
        draft.ApplyGlobalDiscount(discount);
        if (invoiceDate.HasValue) draft.ChangeDate(invoiceDate.Value);

        var invoice = id.HasValue
            ? await invoices.GetByIdWithDetailsAsync(id.Value)
                ?? throw new KeyNotFoundException("Invoice not found.")
            : draft;
        var customer = customerId.HasValue
            ? await customers.GetByIdAsync(customerId.Value)
                ?? throw new KeyNotFoundException("Customer not found.")
            : null;
        var oldCustomer = id.HasValue && invoice.CustomerId.HasValue
            ? await customers.GetByIdAsync(invoice.CustomerId.Value) : null;
        var oldAmount = id.HasValue ? invoice.FinalAmount : 0;
        var oldQuantities = id.HasValue
            ? invoice.Items.ToDictionary(i => i.ProductId, i => i.Quantity)
            : new Dictionary<int, int>();
        var newQuantities = items.ToDictionary(i => i.ProductId, i => i.Quantity);
        var changes = new List<(Product Product, int Delta)>();
        foreach (var productId in oldQuantities.Keys.Union(newQuantities.Keys).Order())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var product = await products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");
            var delta = newQuantities.GetValueOrDefault(productId) - oldQuantities.GetValueOrDefault(productId);
            if (delta > product.StockQuantity)
                throw new InvalidOperationException($"Insufficient stock for '{product.Name}'.");
            changes.Add((product, delta));
        }

        foreach (var (product, delta) in changes)
        {
            if (delta > 0) product.DecreaseStock(delta);
            else if (delta < 0) product.IncreaseStock(-delta);
        }
        if (id.HasValue)
        {
            invoice.ClearItems();
            foreach (var item in items)
                invoice.AddItem(item.ProductId, item.Quantity, item.UnitPrice, item.DiscountAmount);
            invoice.UpdateHeader(customerId, paymentMethod, remarks, discount, charges);
            if (invoiceDate.HasValue) invoice.ChangeDate(invoiceDate.Value);
        }
        oldCustomer?.DecreaseLoyaltyPoints(oldAmount);
        customer?.AddLoyaltyPoints(invoice.FinalAmount);
        if (!id.HasValue) await invoices.AddAsync(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return invoice.Id;
    }
}

