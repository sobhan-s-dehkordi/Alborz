using Alborz.Application.Features.Customers.Commands;
using Alborz.Application.Features.Customers.Queries;
using Alborz.WinUI;
using Alborz.WinUI.Views.Customers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.Customers;

public partial class CustomersViewModel : Alborz.WinUI.ViewModels.Common.ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<CustomerDto> Customers { get; } = new();

    [ObservableProperty]
    public partial string SearchName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchPhone { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchNationalCode { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCustomerSelected))]
    public partial CustomerDto? SelectedCustomer { get; set; }

    public bool IsCustomerSelected => SelectedCustomer != null;

    public CustomersViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = LoadCustomersAsync();
    }

    [RelayCommand]
    public async Task ShowHistoryAsync()
    {
        if (SelectedCustomer is not { } customer) return;
        try
        {
            ErrorMessage = string.Empty;
            using var scope = _scopeFactory.CreateScope();
            var history = await scope.ServiceProvider.GetRequiredService<IMediator>()
                .Send(new GetCustomerHistoryQuery(customer.Id));
            var lines = new System.Collections.Generic.List<string>();
            foreach (var item in history)
                lines.Add($"#{item.InvoiceId} | {item.Date:yyyy-MM-dd} | {item.TotalAmount:N2} | {item.PaymentMethod}");
            if (lines.Count == 0) lines.Add("No purchases recorded.");
            var app = (App)Microsoft.UI.Xaml.Application.Current;
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = $"Purchase history — {customer.Name}",
                Content = new Microsoft.UI.Xaml.Controls.ListView { ItemsSource = lines, MaxHeight = 400 },
                CloseButtonText = "Close",
                XamlRoot = app.AppWindow.Content.XamlRoot
            };
            await App.ShowDialogAsync(dialog);
        }
        catch (Exception ex) { ReportError(ex); }
    }
    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetCustomersQuery(SearchName, SearchPhone, SearchNationalCode);

        var results = await mediator.Send(query);

        Customers.Clear();
        foreach (var c in results)
        {
            Customers.Add(c);
        }

        SelectedCustomer = null;
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    [RelayCommand]
    public async Task ShowAddDialogAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        if (Microsoft.UI.Xaml.Application.Current is App app 
            && app.AppWindow != null)
        {
            var dialog = new Alborz.WinUI.Views.Customers.CustomerDialog
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            var result = await App.ShowDialogAsync(dialog);

            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new CreateCustomerCommand(dialog.CustomerName, dialog.PhoneNumber, dialog.NationalCode);
                await mediator.Send(command);

                await LoadCustomersAsync();
            }
        }
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    [RelayCommand]
    public async Task ShowEditDialogAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        if (SelectedCustomer == null) return;

        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            var dialog = new CustomerDialog(SelectedCustomer)
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            var result = await App.ShowDialogAsync(dialog);

            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new UpdateCustomerCommand(
                    dialog.EditingCustomerId ?? throw new InvalidOperationException("Customer ID is missing."),
                    dialog.CustomerName,
                    dialog.PhoneNumber,
                    dialog.NationalCode);

                await mediator.Send(command);

                await LoadCustomersAsync();
            }
        }
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }
}







