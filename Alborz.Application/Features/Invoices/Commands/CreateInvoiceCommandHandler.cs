using Alborz.Application.Contracts;
using Alborz.Domain.Entities;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public class CreateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        if (!request.Items.Any())
            throw new ArgumentException("The invoice must contain at least one item.");

        var invoice = new Invoice(
            request.CustomerId,
            request.PaymentMethod,
            request.Remarks,
            request.AdditionalCharges
        );

        foreach (var itemDto in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {itemDto.ProductId} was not found.");

            product.DecreaseStock(itemDto.Quantity);
            invoice.AddItem(
            itemDto.ProductId, 
            itemDto.Quantity, 
            itemDto.UnitPrice, 
            itemDto.DiscountAmount);
        }

        if (request.CustomerId.HasValue)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
            if (customer != null)
            {
                customer.AddLoyaltyPoints(invoice.FinalAmount);
            }
        }

        if (request.GlobalDiscount > 0)
        {
            invoice.ApplyGlobalDiscount(request.GlobalDiscount);
        }

        await _invoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}