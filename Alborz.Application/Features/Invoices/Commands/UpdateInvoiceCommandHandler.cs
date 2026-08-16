using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public class UpdateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            throw new ArgumentException("The invoice must contain at least one item.");

        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(request.Id);
        if (invoice == null)
            throw new KeyNotFoundException($"Invoice with ID {request.Id} was not found.");

        var oldCustomerId = invoice.CustomerId;
        var oldFinalAmount = invoice.FinalAmount;

        var oldQuantities = invoice.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var newQuantities = request.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

        var allProductIds = oldQuantities.Keys.Union(newQuantities.Keys).Distinct();

        foreach (var productId in allProductIds)
        {
            int oldQty = oldQuantities.TryGetValue(productId, out var o) ? o : 0;
            int newQty = newQuantities.TryGetValue(productId, out var n) ? n : 0;

            int diff = newQty - oldQty;
            if (diff == 0) continue;

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException($"Product ID {productId} not found.");

            if (diff > 0)
            {
                if (product.StockQuantity < diff)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{product.Name}'. " +
                        $"You want to add {diff} more items, but only {product.StockQuantity} is available.");
                }
                product.DecreaseStock(diff);
            }
            else
            {
                int amountToReturn = Math.Abs(diff);
                product.IncreaseStock(amountToReturn);
            }
        }

        invoice.ClearItems();
        invoice.UpdateHeader(
            request.CustomerId,
            request.PaymentMethod,
            request.Remarks,
            request.GlobalDiscount,
            request.AdditionalCharges
        );

        foreach (var itemDto in request.Items)
        {
            invoice.AddItem(
                itemDto.ProductId,
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemDto.DiscountAmount
            );
        }

        var newFinalAmount = invoice.FinalAmount;
        var newCustomerId = request.CustomerId;

        if (oldCustomerId == newCustomerId)
        {
            if (oldCustomerId.HasValue)
            {
                var customer = await _customerRepository.GetByIdAsync(oldCustomerId.Value);
                if (customer != null)
                {
                    decimal diffAmount = newFinalAmount - oldFinalAmount;

                    if (diffAmount > 0)
                    {
                        customer.AddLoyaltyPoints(diffAmount);
                    }
                    else if (diffAmount < 0)
                    {
                        customer.DecreaseLoyaltyPoints(Math.Abs(diffAmount));
                    }
                }
            }
        }
        else
        {
            if (oldCustomerId.HasValue)
            {
                var oldCustomer = await _customerRepository.GetByIdAsync(oldCustomerId.Value);
                if (oldCustomer != null)
                {
                    oldCustomer.DecreaseLoyaltyPoints(oldFinalAmount);
                }
            }

            if (newCustomerId.HasValue)
            {
                var newCustomer = await _customerRepository.GetByIdAsync(newCustomerId.Value);
                if (newCustomer != null)
                {
                    newCustomer.AddLoyaltyPoints(newFinalAmount);
                }
            }
        }

        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}