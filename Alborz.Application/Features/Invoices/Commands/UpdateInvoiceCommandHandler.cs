using Alborz.Application.Features.Invoices.Services;
using MediatR;

namespace Alborz.Application.Features.Invoices.Commands;

public sealed class UpdateInvoiceCommandHandler(InvoiceWriter writer) : IRequestHandler<UpdateInvoiceCommand>
{
    public async Task Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken) =>
        await writer.SaveAsync(request.Id, request.CustomerId, request.PaymentMethod, request.GlobalDiscount,
            request.AdditionalCharges, request.Remarks, request.Items, cancellationToken, request.InvoiceDate);
}

