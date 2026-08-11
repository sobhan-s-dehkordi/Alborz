using Alborz.Application.Features.Parties.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Application.Features.PurchaseReceipts.Queries;
using Alborz.WinUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectName.WinUI.ViewModels;

public partial class PurchaseHistoryViewModel : ObservableObject
{
    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region <Collections>

    public ObservableCollection<PurchaseReceiptDto> Receipts { get; } = new();

    public ObservableCollection<PartyDto> SupplierSearchResults { get; } = new();

    #endregion

    #region <Observable & Computed Properties>

    [ObservableProperty]
    private PartyDto? _selectedSupplier;

    [ObservableProperty]
    private string _searchSupplierText = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _fromDate;

    [ObservableProperty]
    private DateTimeOffset? _toDate;

    [ObservableProperty]
    private string _referenceNumberFilter = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReceiptSelected))]
    private PurchaseReceiptDto? _selectedReceipt;

    public bool IsReceiptSelected => SelectedReceipt != null;

    public decimal TotalAmountSummary => Receipts.Sum(r => r.TotalAmount);

    #endregion

    #region <Constructor>

    public PurchaseHistoryViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    #endregion

    #region <Commands & Methods>

    [RelayCommand]
    public async Task SearchSuppliersAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
        {
            SupplierSearchResults.Clear();
            SelectedSupplier = null;
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var results = await mediator.Send(new SearchSupplierSuggestQuery(searchText));

        SupplierSearchResults.Clear();
        foreach (var item in results)
        {
            SupplierSearchResults.Add(item);
        }
    }

    [RelayCommand]
    public async Task SearchReceiptsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        int? supplierId = SelectedSupplier?.Id;

        var query = new GetPurchaseReceiptsQuery(
            supplierId,
            FromDate?.DateTime,
            ToDate?.DateTime,
            ReferenceNumberFilter);

        var results = await mediator.Send(query);

        Receipts.Clear();

        if (results != null)
        {
            foreach (var item in results)
            {
                Receipts.Add(item);
            }
        }

        SelectedReceipt = null;

        OnPropertyChanged(nameof(TotalAmountSummary));
    }

    [RelayCommand]
    public async Task EditReceiptAsync()
    {
        if (SelectedReceipt == null) return;

        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            string uniqueTag = $"PurchaseReceipt_Edit_{SelectedReceipt.Id}";
            string header = $"Edit Receipt #{SelectedReceipt.Id}";

            app.AppWindow.OpenOrFocusTab(
                header,
                typeof(Alborz.WinUI.Views.PurchaseReceipts.PurchaseReceiptPage),
                null,
                uniqueTag,
                SelectedReceipt.Id);
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task ViewDetailsAsync()
    {
        if (SelectedReceipt == null) return;

        if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
        {
            var dialog = new Alborz.WinUI.Views.PurchaseReceipts.ReceiptDetailsDialog(SelectedReceipt)
            {
                XamlRoot = app.AppWindow.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    public async Task ExportToExcelAsync()
    {
        if (!Receipts.Any()) return;

        // کدهای مربوط به خروجی اکسل (می‌توانید از کتابخانه‌هایی مثل ClosedXML استفاده کنید)
        await Task.CompletedTask;
    }

    #endregion
}