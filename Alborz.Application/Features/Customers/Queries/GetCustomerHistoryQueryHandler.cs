using Alborz.Application.Contracts;
using MediatR;

namespace Alborz.Application.Features.Customers.Queries;

public class GetCustomerHistoryQueryHandler(IInvoiceRepository invoiceRepository) : IRequestHandler<GetCustomerHistoryQuery, List<CustomerHistoryDto>>
{
    private readonly IInvoiceRepository _invoiceRepository = invoiceRepository;

    public async Task<List<CustomerHistoryDto>> Handle(GetCustomerHistoryQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetByCustomerIdAsync(request.CustomerId);

        return invoices.Select(i => new CustomerHistoryDto(
            i.Id,
            i.InvoiceDate,
            i.FinalAmount,
            i.PaymentMethod.ToString()
        )).OrderByDescending(x => x.Date).ToList();
    }
}