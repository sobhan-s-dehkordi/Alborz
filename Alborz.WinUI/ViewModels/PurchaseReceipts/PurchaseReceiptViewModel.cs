using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Alborz.Application.Features.Parties.Queries;
using Alborz.Application.Features.Products.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;

namespace ProjectName.WinUI.ViewModels;

public partial class PurchaseReceiptViewModel : ObservableObject
{

    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;
    private ProductDto? _selectedProductFromSearch;

    #endregion

    #region <Collections>

    public ObservableCollection<ReceiptItemUIModel> ReceiptItems { get; } = new();
    public ObservableCollection<PartyDto> Suppliers { get; } = new();
    public ObservableCollection<ProductDto> ProductSearchResults { get; } = new();
    public ObservableCollection<PartyDto> SupplierSearchResults { get; } = new();

    #endregion

    #region <Observable & Computed Properties>

    [ObservableProperty]
    private PartyDto? _selectedSupplier;

    [ObservableProperty]
    private string _searchSupplierText = string.Empty;

    [ObservableProperty]
    private string _searchProductText = string.Empty;

    [ObservableProperty]
    private DateTimeOffset _receiptDate = DateTimeOffset.Now;

    [ObservableProperty]
    private string _referenceNumber = string.Empty;

    [ObservableProperty]
    private string _remarks = string.Empty;

    [ObservableProperty]
    private int _inputQuantity = 1;

    [ObservableProperty]
    private string _inputPrice = "0";

    [ObservableProperty]
    private string _inputItemDiscount = "0";

    [ObservableProperty]
    private string _inputDiscountPercentage = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(OverallTotalDiscount))]
    private string _totalDiscount = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(AdditionalChargesValue))]
    private string _additionalCharges = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public decimal AdditionalChargesValue => decimal.TryParse(AdditionalCharges.Replace(",", ""), out var ac) ? ac : 0;

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

    #endregion

    #region <Constructor>

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
    public async Task SearchProductsAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || searchText.Length < 2)
        {
            ProductSearchResults.Clear();
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var results = await mediator.Send(new SearchProductSuggestQuery(searchText));

        ProductSearchResults.Clear();
        foreach (var item in results)
        {
            ProductSearchResults.Add(item);
        }
    }

    public void SelectProduct(ProductDto product)
    {
        _selectedProductFromSearch = product;
        if (product is not null)
        {
            SearchProductText = product.Name;
            InputPrice = product.PurchasePrice.ToString("N0");
            InputQuantity = 1;
            InputItemDiscount = "0";
        }
    }

    [RelayCommand]
    public void AddToReceipt()
    {
        ErrorMessage = string.Empty;

        if (!int.TryParse(InputDiscountPercentage, out int discountPercent))
        {
            discountPercent = 0;
        }

        if (discountPercent < 0 || discountPercent > 100 || discountPercent % 5 != 0)
        {
            ErrorMessage = "Discount must be a multiple of 5 (e.g., 0, 5, 10, 15) and between 0 and 100.";
            return;
        }

        if (_selectedProductFromSearch == null) return;

        if (InputQuantity <= 0) return;
        if (!decimal.TryParse(InputPrice.Replace(",", ""), out decimal price)) return;

        decimal totalLinePrice = price * InputQuantity;
        decimal calculatedDiscountAmount = totalLinePrice * (decimal)(Convert.ToInt32(InputDiscountPercentage) / 100.0);

        var newItem = new ReceiptItemUIModel
        {
            ProductId = _selectedProductFromSearch.Id,
            ProductName = _selectedProductFromSearch.Name,
            Quantity = InputQuantity,
            UnitPrice = price,
            DiscountAmount = calculatedDiscountAmount
        };

        ReceiptItems.Add(newItem);

        _selectedProductFromSearch = null;
        SearchProductText = string.Empty;
        InputQuantity = 1;
        InputPrice = "0";
        InputDiscountPercentage = "0";

        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(TotalLineDiscounts));
        OnPropertyChanged(nameof(NetAmount));
    }

    [RelayCommand]
    public void RemoveItem(ReceiptItemUIModel item)
    {
        if (item != null && ReceiptItems.Contains(item))
        {
            ReceiptItems.Remove(item);

            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalLineDiscounts));
            OnPropertyChanged(nameof(NetAmount));
        }
    }

    [RelayCommand]
    public async Task SaveReceiptAsync()
    {
        if (!ReceiptItems.Any() || SelectedSupplier == null) return;

        decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var td) ? td : 0;

        var itemsDto = ReceiptItems.Select(x => new PurchaseItemDto(x.ProductId, x.Quantity, x.UnitPrice, x.DiscountAmount)).ToList();

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

    #endregion
}
