namespace Alborz.Application.Features.Reports.Queries;

public sealed record ReportRow(int Id, string Name, DateTime? Date, string Description,
    int Quantity, decimal Amount, int? Balance = null);
