using Alborz.Application.Features.Reports.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.Reports;

public partial class ReportsViewModel(IServiceScopeFactory scopeFactory) : ObservableObject
{
    public ReportKind Kind { get; set; }
    public ObservableCollection<ReportRow> Rows { get; } = new();
    [ObservableProperty] public partial DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now.AddMonths(-1);
    [ObservableProperty] public partial DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now;
    [ObservableProperty] public partial string ProductId { get; set; } = string.Empty;
    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;
    [ObservableProperty] public partial bool IsBusy { get; set; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        Rows.Clear();
        try
        {
            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var rows = await mediator.Send(new GetReportQuery(Kind, FromDate?.DateTime, ToDate?.DateTime,
                int.TryParse(ProductId, out var id) ? id : null));
            foreach (var row in rows) Rows.Add(row);
            if (Rows.Count == 0) ErrorMessage = "No records match these filters.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex is ArgumentException or System.Collections.Generic.KeyNotFoundException
                ? ex.Message : "Could not load the report. Check the database connection and retry.";
        }
        finally { IsBusy = false; }
    }
}

