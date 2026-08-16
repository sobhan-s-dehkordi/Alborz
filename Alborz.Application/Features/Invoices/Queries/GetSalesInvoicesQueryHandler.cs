using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Invoices.Queries;

public class GetSalesInvoicesQueryHandler : IRequestHandler<GetSalesInvoicesQuery, List<SalesInvoiceDto>>
{
    private readonly IInvoiceRepository _repository;

    public GetSalesInvoicesQueryHandler(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SalesInvoiceDto>> Handle(GetSalesInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _repository.GetFilteredInvoicesAsync(
            request.CustomerId,
            request.FromDate,
            request.ToDate,
            request.InvoiceId);

        return invoices.Select(i => new SalesInvoiceDto(
            i.Id,
            i.InvoiceDate,
            i.Customer?.Name ?? "Walk-in Customer",
            i.Remarks ?? string.Empty,
            i.TotalAmount,
            i.DiscountAmount,
            i.AdditionalCharges,
            i.FinalAmount
        )).ToList();
    }
}