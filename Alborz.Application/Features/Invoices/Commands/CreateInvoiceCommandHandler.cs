using Alborz.Application.Features.Invoices.Services;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public sealed class CreateInvoiceCommandHandler(InvoiceWriter writer) : IRequestHandler<CreateInvoiceCommand, int>
{
    public Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken) =>
        writer.SaveAsync(null, request.CustomerId, request.PaymentMethod, request.GlobalDiscount,
            request.AdditionalCharges, request.Remarks, request.Items, cancellationToken, request.InvoiceDate);
}

