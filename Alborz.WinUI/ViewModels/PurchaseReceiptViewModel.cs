using Alborz.Application.Features.Products.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectName.WinUI.ViewModels;

public partial class PurchaseReceiptViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<ReceiptItemUIModel> ReceiptItems { get; } = new();
    public ObservableCollection<PartyDto> Suppliers { get; } = new();
    public ObservableCollection<ProductDto> ProductSearchResults { get; } = new();

    // 1. Add these new properties
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(AdditionalChargesValue))]
    private string _additionalCharges = "0";

    [ObservableProperty]
    private string _remarks = string.Empty;

    // 2. Add a helper to convert the formatted string back to decimal for the UI summary
    public decimal AdditionalChargesValue => decimal.TryParse(AdditionalCharges.Replace(",", ""), out var ac) ? ac : 0;


    [ObservableProperty] private PartyDto? _selectedSupplier;
    [ObservableProperty] private DateTimeOffset _receiptDate = DateTimeOffset.Now;
    [ObservableProperty] private string _referenceNumber = string.Empty;

    [ObservableProperty] private string _searchProductText = string.Empty;
    private ProductDto? _selectedProductFromSearch;

    [ObservableProperty] private int _inputQuantity = 1;
    [ObservableProperty] private string _inputPrice = "0";
    [ObservableProperty] private string _inputItemDiscount = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(OverallTotalDiscount))]
    private string _totalDiscount = "0";

    public decimal TotalAmount => ReceiptItems.Sum(x => x.TotalPrice);

    public decimal TotalLineDiscounts => ReceiptItems.Sum(x => x.DiscountAmount);

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
            return TotalAmount - globalDiscount + AdditionalChargesValue;
        }
    }

    public PurchaseReceiptViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        ReceiptItems.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(NetAmount));
            OnPropertyChanged(nameof(TotalLineDiscounts));
            OnPropertyChanged(nameof(OverallTotalDiscount));
        };

        _ = LoadSuppliersAsync();
    }

    private async Task LoadSuppliersAsync()
    {
        // var query = new GetSuppliersQuery(); ...
    }


    [RelayCommand]
    public async Task SearchProductsAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2) return;

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        int? code = int.TryParse(searchText, out int c) ? c : null;
        var query = new GetProductsQuery(code, code, searchText, searchText);

        var results = await mediator.Send(query);

        ProductSearchResults.Clear();
        foreach (var item in results) ProductSearchResults.Add(item);
    }

    public void SelectProduct(ProductDto product)
    {
        _selectedProductFromSearch = product;
        SearchProductText = product.Name;
        InputPrice = product.PurchasePrice.ToString("N0");
        InputQuantity = 1;
        InputItemDiscount = "0";
    }

    [RelayCommand]
    public void AddToReceipt()
    {
        if (_selectedProductFromSearch == null) return;

        decimal price = decimal.TryParse(InputPrice.Replace(",", ""), out var p) ? p : 0;
        decimal discount = decimal.TryParse(InputItemDiscount.Replace(",", ""), out var d) ? d : 0;

        ReceiptItems.Add(new ReceiptItemUIModel
        {
            ProductId = _selectedProductFromSearch.Id,
            ProductName = _selectedProductFromSearch.Name,
            Quantity = InputQuantity,
            UnitPrice = price,
            DiscountAmount = discount
        });

        SearchProductText = string.Empty;
        _selectedProductFromSearch = null;
        InputQuantity = 1;
        InputPrice = "0";
        InputItemDiscount = "0";

        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(NetAmount));
    }


    [RelayCommand]
    public async Task SaveReceiptAsync()
    {
        if (!ReceiptItems.Any() || SelectedSupplier == null) return;

        decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var td) ? td : 0;

        var itemsDto = ReceiptItems.Select(x => new PurchaseItemDto(x.ProductId, x.Quantity, x.UnitPrice, x.DiscountAmount)).ToList();

        // Pass the new fields
        var command = new CreatePurchaseReceiptCommand(
            SelectedSupplier.Id,
            ReceiptDate.DateTime,
            ReferenceNumber,
            globalDiscount,
            AdditionalChargesValue,
            Remarks,
            itemsDto);

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(command);

        // Reset UI
        ReceiptItems.Clear();
        ReferenceNumber = string.Empty;
        TotalDiscount = "0";
        AdditionalCharges = "0";
        Remarks = string.Empty;
    }
}