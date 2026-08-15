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

namespace ProjectName.WinUI.ViewModels;

public partial class CustomersViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<CustomerDto> Customers { get; } = new();

    [ObservableProperty]
    private string _searchName = string.Empty;

    [ObservableProperty]
    private string _searchPhone = string.Empty;

    [ObservableProperty]
    private string _searchNationalCode = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCustomerSelected))]
    private CustomerDto? _selectedCustomer;

    public bool IsCustomerSelected => SelectedCustomer != null;

    public CustomersViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = LoadCustomersAsync();
    }

    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
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

    [RelayCommand]
    public async Task ShowAddDialogAsync()
    {
        if (Microsoft.UI.Xaml.Application.Current is App app 
            && app.AppWindow != null)
        {
            var dialog = new Alborz.WinUI.Views.Customers.CustomerDialog
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

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

    [RelayCommand]
    public async Task ShowEditDialogAsync()
    {
        if (SelectedCustomer == null) return;

        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            var dialog = new CustomerDialog(SelectedCustomer)
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var command = new UpdateCustomerCommand(
                    dialog.EditingCustomerId.Value,
                    dialog.CustomerName,
                    dialog.PhoneNumber,
                    dialog.NationalCode);

                await mediator.Send(command);

                await LoadCustomersAsync();
            }
        }
    }
}
