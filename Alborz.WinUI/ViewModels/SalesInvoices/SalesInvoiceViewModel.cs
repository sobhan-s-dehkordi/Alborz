using Alborz.Application.Features.Customers.Commands;
using Alborz.Application.Features.Customers.Queries;
using Alborz.Application.Features.Invoices.Commands;
using Alborz.Application.Features.Invoices.Queries;
using Alborz.Application.Features.Products.Queries;
using Alborz.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.SalesInvoices;

public partial class SalesInvoiceViewModel : Alborz.WinUI.ViewModels.Common.ViewModelBase
{
    private readonly IServiceScopeFactory _scopeFactory;
    private ProductDto? _selectedProductFromSearch;
    private int? _editingInvoiceId;

    public ObservableCollection<InvoiceItemUIModel> InvoiceItems { get; } = new();
    public ObservableCollection<CustomerDto> CustomerSearchResults { get; } = new();
    public ObservableCollection<ProductDto> ProductSearchResults { get; } = new();
    public ObservableCollection<PaymentMethod> PaymentMethods { get; } = new(Enum.GetValues<PaymentMethod>());

    // --- ??? ?????? ---
    [ObservableProperty] public partial CustomerDto? SelectedCustomer { get; set; }
    [ObservableProperty] public partial string SearchCustomerText { get; set; } = string.Empty;
    [ObservableProperty] public partial string SearchProductText { get; set; } = string.Empty;

    // --- ??????? ??? ???? ????? ---
    [ObservableProperty] public partial string NewCustomerName { get; set; } = string.Empty;
    [ObservableProperty] public partial string NewCustomerPhone { get; set; } = string.Empty;
    [ObservableProperty] public partial string NewCustomerNationalCode { get; set; } = string.Empty;

    // --- ??????? ??? ???? ???? ---
    [ObservableProperty] public partial string NewProductName { get; set; } = string.Empty;
    [ObservableProperty] public partial string NewProductSellPrice { get; set; } = string.Empty;

    // --- ??????? ???? ?????? ---
    [ObservableProperty] public partial DateTimeOffset InvoiceDate { get; set; } = DateTimeOffset.Now;
    [ObservableProperty] public partial PaymentMethod SelectedPaymentMethod { get; set; } = PaymentMethod.Cash;
    [ObservableProperty] public partial string Remarks { get; set; } = string.Empty;

    // --- ?????? ????? ????? ---
    [ObservableProperty] public partial int InputQuantity { get; set; } = 1;
    [ObservableProperty] public partial string InputPrice { get; set; } = "0";
    [ObservableProperty] public partial string InputDiscountPercentage { get; set; } = "0";

    // --- ????? ?? ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(OverallTotalDiscount))]
    public partial string TotalDiscount { get; set; } = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(AdditionalChargesValue))]
    public partial string AdditionalCharges { get; set; } = "0";

    [ObservableProperty] public partial string SubmitButtonText { get; set; } = "Submit Sales Invoice";



    public decimal AdditionalChargesValue => decimal.TryParse(AdditionalCharges.Replace(",", ""), out var ac) ? ac : 0;
    public decimal TotalAmount => InvoiceItems.Sum(x => (x.Quantity * x.UnitPrice));
    public decimal TotalLineDiscounts => InvoiceItems.Sum(x => x.DiscountAmount);

    public decimal OverallTotalDiscount
    {
        get
        {
            decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var d) ? d : 0;
            return TotalLineDiscounts + globalDiscount;
        }
    }

    public decimal NetAmount
    {
        get
        {
            decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var d) ? d : 0;
            return TotalAmount - globalDiscount + AdditionalChargesValue - TotalLineDiscounts;
        }
    }

    public SalesInvoiceViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        InvoiceItems.CollectionChanged += (s, e) =>
        {            if (e.OldItems != null)
                foreach (System.ComponentModel.INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= OnLineChanged;
            if (e.NewItems != null)
                foreach (System.ComponentModel.INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += OnLineChanged;
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalLineDiscounts));
            OnPropertyChanged(nameof(OverallTotalDiscount));
            OnPropertyChanged(nameof(NetAmount));
        };
    }

    private void OnLineChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(TotalLineDiscounts));
        OnPropertyChanged(nameof(OverallTotalDiscount));
        OnPropertyChanged(nameof(NetAmount));
    }
    public async Task InitializeAsync(int? invoiceId)
    {
        try
        {
            ErrorMessage = string.Empty;
        _editingInvoiceId = invoiceId;

        if (invoiceId.HasValue)
        {
            SubmitButtonText = "Update Sales Invoice";
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var invoiceDetail = await mediator.Send(new GetSalesInvoiceByIdQuery(invoiceId.Value));

            if (invoiceDetail != null)
            {
                if (invoiceDetail.CustomerId.HasValue)
                {
                    SelectedCustomer = new CustomerDto(
                        invoiceDetail.CustomerId.Value,
                        invoiceDetail.CustomerName ?? "",
                        invoiceDetail.CustomerPhone ?? "",
                        "", 0, 0);

                    SearchCustomerText = invoiceDetail.CustomerName ?? "";
                }

                InvoiceDate = invoiceDetail.InvoiceDate;
                SelectedPaymentMethod = invoiceDetail.PaymentMethod;
                Remarks = invoiceDetail.Remarks;
                TotalDiscount = invoiceDetail.GlobalDiscount.ToString("N0");
                AdditionalCharges = invoiceDetail.AdditionalCharges.ToString("N0");

                InvoiceItems.Clear();
                foreach (var item in invoiceDetail.Items)
                {
                    InvoiceItems.Add(new InvoiceItemUIModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = item.DiscountAmount
                    });
                }
            }
        }
        else
        {
            SubmitButtonText = "Submit Sales Invoice";
            InvoiceDate = DateTimeOffset.Now;
            SelectedPaymentMethod = PaymentMethod.Cash;
            InvoiceItems.Clear();
            SelectedCustomer = null;
            SearchCustomerText = string.Empty;
        }
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    // --- ?????? ????? ---
    [RelayCommand]
    public async Task SearchCustomersAsync(string searchText)
    {
        try
        {
            ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
        {
            CustomerSearchResults.Clear();
            SelectedCustomer = null;
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var query = new GetCustomersQuery(searchText, null, null);
        var results = await mediator.Send(query);

        CustomerSearchResults.Clear();
        foreach (var item in results) CustomerSearchResults.Add(item);
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    // --- ??? ???? ????? ---
    [RelayCommand]
    public async Task QuickRegisterCustomerAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(NewCustomerName) || string.IsNullOrWhiteSpace(NewCustomerPhone)) return;

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateCustomerCommand(NewCustomerName, NewCustomerPhone, NewCustomerNationalCode);
        int newCustomerId = await mediator.Send(command);

        var newCustomer = new CustomerDto(newCustomerId, NewCustomerName, NewCustomerPhone, NewCustomerNationalCode, 0, 0);

        SelectedCustomer = newCustomer;
        SearchCustomerText = newCustomer.Name;

        NewCustomerName = string.Empty;
        NewCustomerPhone = string.Empty;
        NewCustomerNationalCode = string.Empty;
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    // --- ?????? ???? ---
    [RelayCommand]
    public async Task SearchProductsAsync(string searchText)
    {
        try
        {
            ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
        {
            ProductSearchResults.Clear();
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var results = await mediator.Send(new SearchProductSuggestQuery(searchText));

        ProductSearchResults.Clear();
        foreach (var item in results) ProductSearchResults.Add(item);
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    public void SelectProduct(ProductDto? product)
    {
        _selectedProductFromSearch = product;
        if (product != null)
        {
            SearchProductText = product.Name;
            InputPrice = product.SellPrice.ToString("N0");
            InputQuantity = 1;
            InputDiscountPercentage = "0";
        }
    }

    [RelayCommand]
    public void AddToInvoice()
    {
        ErrorMessage = string.Empty;

        if (!decimal.TryParse(InputDiscountPercentage, out var discountPercent) || discountPercent < 0 || discountPercent > 100)
        {
            ErrorMessage = "Discount must be between 0 and 100 percent.";
            return;
        }
        if (_selectedProductFromSearch == null || InputQuantity <= 0) return;
        if (!decimal.TryParse(InputPrice.Replace(",", ""), out decimal price) || price < 0 || price > 9999999999999999.99m / InputQuantity) { ErrorMessage = "Enter a valid non-negative price."; return; }

        decimal totalLinePrice = price * InputQuantity;
        decimal calculatedDiscountAmount = decimal.Round(totalLinePrice * discountPercent / 100m, 2);

        var newItem = new InvoiceItemUIModel
        {
            ProductId = _selectedProductFromSearch.Id,
            ProductName = _selectedProductFromSearch.Name,
            Quantity = InputQuantity,
            UnitPrice = price,
            DiscountAmount = calculatedDiscountAmount
        };

        InvoiceItems.Add(newItem);

        _selectedProductFromSearch = null;
        SearchProductText = string.Empty;
        InputQuantity = 1;
        InputDiscountPercentage = "0";
    }

    [RelayCommand]
    public void RemoveItem(InvoiceItemUIModel item)
    {
        if (item != null && InvoiceItems.Contains(item)) InvoiceItems.Remove(item);
    }

    [RelayCommand]
    public async Task SaveInvoiceAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        ErrorMessage = string.Empty;

        if (!InvoiceItems.Any())
        {
            ErrorMessage = "Cannot save an empty invoice.";
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var d) ? d : 0;

        var itemsDto = InvoiceItems.Select(i => new InvoiceItemDto(
            i.ProductId,
            i.Quantity,
            i.UnitPrice,
            i.DiscountAmount
        )).ToList();

        if (_editingInvoiceId.HasValue)
        {
            var command = new UpdateInvoiceCommand(
                _editingInvoiceId.Value,
                SelectedCustomer?.Id,
                SelectedPaymentMethod,
                globalDiscount,
                AdditionalChargesValue,
                Remarks,
                itemsDto, InvoiceDate.DateTime
            );

            await mediator.Send(command);

            if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
            {
                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = "Success",
                    Content = $"Sales invoice #{_editingInvoiceId.Value} has been updated successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = app.AppWindow.Content.XamlRoot
                };
                await App.ShowDialogAsync(dialog);

                string uniqueTag = $"SalesInvoice_Edit_{_editingInvoiceId.Value}";
                app.AppWindow.CloseTab(uniqueTag);
            }
        }
        else
        {
            var command = new CreateInvoiceCommand(
                SelectedCustomer?.Id,
                SelectedPaymentMethod,
                globalDiscount,
                AdditionalChargesValue,
                Remarks,
                itemsDto, InvoiceDate.DateTime
            );

            int newInvoiceId = await mediator.Send(command);

            if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
            {
                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = "Success",
                    Content = $"Sales invoice #{newInvoiceId} has been saved successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = app.AppWindow.Content.XamlRoot
                };
                await App.ShowDialogAsync(dialog);
            }

            InvoiceItems.Clear();
            SelectedCustomer = null;
            SearchCustomerText = string.Empty;
            TotalDiscount = "0";
            AdditionalCharges = "0";
            Remarks = string.Empty;
        }
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }
}




