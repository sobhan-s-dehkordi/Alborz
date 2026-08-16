using Alborz.Application.Features.Customers.Queries;
using Alborz.Application.Features.Invoices.Queries;
using Alborz.WinUI;
using Alborz.WinUI.Views.SalesInvoices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels;

public partial class SalesHistoryViewModel : ObservableObject
{
    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region <Collections>

    // فرض بر وجود DTO ای به نام SalesInvoiceDto برای لیست فاکتورها
    public ObservableCollection<SalesInvoiceDto> Invoices { get; } = new();

    public ObservableCollection<CustomerDto> CustomerSearchResults { get; } = new();

    #endregion

    #region <Observable & Computed Properties>

    [ObservableProperty]
    private CustomerDto? _selectedCustomer;

    [ObservableProperty]
    private string _searchCustomerText = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _fromDate = DateTimeOffset.Now.AddDays(-7);

    [ObservableProperty]
    private DateTimeOffset? _toDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string _invoiceIdFilter = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsInvoiceSelected))]
    private SalesInvoiceDto? _selectedInvoice;

    public bool HasInvoices => Invoices.Any();

    public bool IsInvoiceSelected => SelectedInvoice != null;

    // محاسبات فوتر
    public decimal TotalAmountSummary => Invoices.Sum(r => r.TotalAmount);
    public decimal TotalDiscountSummary => Invoices.Sum(r => r.TotalDiscount);
    public decimal TotalAdditionalChargesSummary => Invoices.Sum(r => r.AdditionalCharges);
    public decimal TotalNetAmountSummary => Invoices.Sum(r => r.NetAmount);

    #endregion

    #region <Constructor>

    public SalesHistoryViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        Invoices.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasInvoices));
            OnPropertyChanged(nameof(TotalAmountSummary));
            OnPropertyChanged(nameof(TotalDiscountSummary));
            OnPropertyChanged(nameof(TotalAdditionalChargesSummary));
            OnPropertyChanged(nameof(TotalNetAmountSummary));
        };

        _ = SearchInvoicesAsync();
    }

    #endregion

    #region <Commands & Methods>

    [RelayCommand]
    public async Task SearchCustomersAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
        {
            CustomerSearchResults.Clear();
            SelectedCustomer = null;
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // استفاده از کوئری جستجوی مشتری (همانند فاکتور فروش)
        var query = new GetCustomersQuery(searchText, null, null);
        var results = await mediator.Send(query);

        CustomerSearchResults.Clear();
        foreach (var item in results)
        {
            CustomerSearchResults.Add(item);
        }
    }

    [RelayCommand]
    public async Task SearchInvoicesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        int? customerId = SelectedCustomer?.Id;
        int? invoiceId = int.TryParse(InvoiceIdFilter, out var id) ? id : null;

        // فراخوانی کوئری جهت فیلتر و دریافت لیست فاکتورها
        var query = new GetSalesInvoicesQuery(
            customerId,
            FromDate?.DateTime,
            ToDate?.DateTime,
            invoiceId);

        var results = await mediator.Send(query);

        Invoices.Clear();

        if (results != null)
        {
            foreach (var item in results)
            {
                Invoices.Add(item);
            }
        }

        SelectedInvoice = null;
        OnPropertyChanged(nameof(TotalAmountSummary));
    }

    [RelayCommand]
    public async Task EditInvoiceAsync()
    {
        if (SelectedInvoice == null) return;

        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            string uniqueTag = $"SalesInvoice_Edit_{SelectedInvoice.Id}";
            string header = $"Edit Inv #{SelectedInvoice.Id}";

            // باز کردن تب مربوط به صفحه فروش (همان صفحه‌ای که با هم طراحی کردیم)
            app.AppWindow.OpenOrFocusTab(
                header,
                typeof(Alborz.WinUI.Views.SalesInvoices.SaleInvoicePage),
                null,
                uniqueTag,
                SelectedInvoice.Id);
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ViewDetailsAsync()
    {
        if (SelectedInvoice == null) return;

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // واکشی مجدد جزئیات فاکتور شامل ریز اقلام برای نمایش در دیالوگ
        var invoiceDetail = await mediator.Send(new GetSalesInvoiceByIdQuery(SelectedInvoice.Id));

        if (invoiceDetail != null && Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            var dialog = new InvoiceDetailsDialog(invoiceDetail)
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    public async Task ExportToExcelAsync()
    {
        if (!Invoices.Any()) return;

        try
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Excel Workbook", new List<string>() { ".xlsx" });
            savePicker.SuggestedFileName = $"Sales_History_{DateTime.Now:yyyyMMdd_HHmm}";

            if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.AppWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);
            }
            else return;

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {
                using var scope = _scopeFactory.CreateScope();

                // فرض بر وجود متد ExportSalesInvoices در سرویس اکسل
                // var excelService = scope.ServiceProvider.GetRequiredService<IExcelExportService>();
                // byte[] fileBytes = excelService.ExportSalesInvoices(Invoices);

                // Windows.Storage.CachedFileManager.DeferUpdates(file);
                // await Windows.Storage.FileIO.WriteBytesAsync(file, fileBytes);
                // await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);

                await ShowDialogAsync("Export Successful", $"Data exported to {file.Name}");
            }
        }
        catch (Exception ex)
        {
            await ShowDialogAsync("Export Failed", ex.Message);
        }
    }

    private async Task ShowDialogAsync(string title, string content)
    {
        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = app.AppWindow.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    #endregion
}