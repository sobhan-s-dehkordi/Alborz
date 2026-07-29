namespace Alborz.Application.Features.Customers.Queries;

public record CustomerHistoryDto(int InvoiceId, DateTime Date, decimal TotalAmount, string PaymentMethod);
