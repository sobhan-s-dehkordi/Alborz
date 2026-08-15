using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Invoices.Queries;

public record GetSalesInvoiceByIdQuery(int InvoiceId) : IRequest<SalesInvoiceDetailDto?>;

public class GetSalesInvoiceByIdQueryHandler : IRequestHandler<GetSalesInvoiceByIdQuery, SalesInvoiceDetailDto?>
{
    private readonly IInvoiceRepository _repository;

    public GetSalesInvoiceByIdQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<SalesInvoiceDetailDto?> Handle(GetSalesInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _repository.GetByIdWithDetailsAsync(request.InvoiceId);

        if (invoice == null) return null;

        var itemsDto = invoice.Items.Select(i => new SalesInvoiceItemDetailDto(
            i.ProductId,
            i.Product?.Name ?? "Unknown Product",
            i.Quantity,
            i.UnitPrice,
            0
        )).ToList();

        return new SalesInvoiceDetailDto(
            invoice.Id,
            invoice.CustomerId,
            invoice.Customer?.Name,
            invoice.Customer?.PhoneNumber,
            invoice.InvoiceDate,
            invoice.PaymentMethod,
            invoice.Remarks,
            invoice.DiscountAmount,
            invoice.AdditionalCharges,
            itemsDto
        );
    }
}