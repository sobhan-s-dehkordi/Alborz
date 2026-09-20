using Alborz.Application.Features.Parties.Queries;
using Alborz.Application.Features.Products.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Application.Features.PurchaseReceipts.Queries;
using Alborz.WinUI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Alborz.WinUI.ViewModels.PurchaseReceipts;

public partial class PurchaseReceiptViewModel : Alborz.WinUI.ViewModels.Common.ViewModelBase
{

    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;
    private ProductDto? _selectedProductFromSearch;
    private int? _editingReceiptId;

    #endregion

    #region <Collections>

    public ObservableCollection<ReceiptItemUIModel> ReceiptItems { get; } = new();
    public ObservableCollection<PartyDto> Suppliers { get; } = new();
    public ObservableCollection<ProductDto> ProductSearchResults { get; } = new();
    public ObservableCollection<PartyDto> SupplierSearchResults { get; } = new();

    #endregion

    #region <Observable & Computed Properties>

    [ObservableProperty]
    public partial string SubmitButtonText { get; set; } = "Submit Purchase Receipt";

    [ObservableProperty]
    public partial PartyDto? SelectedSupplier { get; set; }

    [ObservableProperty]
    public partial string SearchSupplierText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchProductText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTimeOffset ReceiptDate { get; set; } = DateTimeOffset.Now;

    [ObservableProperty]
    public partial string ReferenceNumber { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Remarks { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int InputQuantity { get; set; } = 1;

    [ObservableProperty]
    public partial string InputPrice { get; set; } = "0";

    [ObservableProperty]
    public partial string InputItemDiscount { get; set; } = "0";

    [ObservableProperty]
    public partial string InputDiscountPercentage { get; set; } = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(OverallTotalDiscount))]
    public partial string TotalDiscount { get; set; } = "0";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NetAmount))]
    [NotifyPropertyChangedFor(nameof(AdditionalChargesValue))]
    public partial string AdditionalCharges { get; set; } = "0";



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
        {            if (e.OldItems != null)
                foreach (System.ComponentModel.INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= OnLineChanged;
            if (e.NewItems != null)
                foreach (System.ComponentModel.INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += OnLineChanged;
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
        try
        {
            ErrorMessage = string.Empty;
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
        catch (System.Exception ex) { ReportError(ex); }
    }

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
        foreach (var item in results)
        {
            ProductSearchResults.Add(item);
        }

        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    public void SelectProduct(ProductDto? product)
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

        if (!decimal.TryParse(InputDiscountPercentage, out var discountPercent) || discountPercent < 0 || discountPercent > 100)
        {
            ErrorMessage = "Discount must be between 0 and 100 percent.";
            return;
        }
        if (_selectedProductFromSearch == null) { ErrorMessage = "Select a product."; return; }


        if (InputQuantity <= 0) { ErrorMessage = "Quantity must be positive."; return; }
        if (!decimal.TryParse(InputPrice.Replace(",", ""), out decimal price) || price < 0 || price > 9999999999999999.99m / InputQuantity) { ErrorMessage = "Enter a valid non-negative price."; return; }

        var existing = ReceiptItems.FirstOrDefault(i => i.ProductId == _selectedProductFromSearch.Id);
        if (existing != null)
        {
            try
            {
                var line = Alborz.Application.Common.DocumentLineEditor.Increase(existing.ProductId,
                    existing.Quantity, existing.UnitPrice, existing.DiscountAmount, InputQuantity);
                existing.Quantity = line.Quantity;
                existing.DiscountAmount = line.DiscountAmount;
                _selectedProductFromSearch = null;
                SearchProductText = string.Empty;
                InputQuantity = 1;
                InputDiscountPercentage = "0";
            }
            catch (Exception ex) { ReportError(ex); }
            return;
        }
        decimal totalLinePrice = price * InputQuantity;
        decimal calculatedDiscountAmount = decimal.Round(totalLinePrice * discountPercent / 100m, 2);

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

    public void EditItem(ReceiptItemUIModel item, int quantity, decimal price, decimal discount)
    {
        if (!ReceiptItems.Contains(item)) throw new InvalidOperationException("This row no longer exists.");
        var line = Alborz.Application.Common.DocumentLineEditor.Validate(item.ProductId, quantity, price, discount);
        item.Quantity = line.Quantity;
        item.UnitPrice = line.UnitPrice;
        item.DiscountAmount = line.DiscountAmount;
    }
    [RelayCommand]
    public async Task SaveReceiptAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
        if (!ReceiptItems.Any() || SelectedSupplier == null) throw new ArgumentException("Select a supplier and add at least one item.");

        decimal globalDiscount = decimal.TryParse(TotalDiscount.Replace(",", ""), out var td) ? td : 0;
        var itemsDto = ReceiptItems.Select(x => new PurchaseItemDto(x.ProductId, x.Quantity, x.UnitPrice, x.DiscountAmount)).ToList();

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        if (_editingReceiptId.HasValue)
        {
            var command = new UpdatePurchaseReceiptCommand(
                _editingReceiptId.Value,
                SelectedSupplier.Id,
                ReceiptDate.DateTime,
                ReferenceNumber,
                globalDiscount,
                AdditionalChargesValue,
                Remarks,
                itemsDto);

            await mediator.Send(command);

            if (Microsoft.UI.Xaml.Application.Current is App app && app.AppWindow != null)
            {
                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = "Success",
                    Content = $"Receipt #{_editingReceiptId.Value} has been updated successfully.",
                    CloseButtonText = "OK",
                    XamlRoot = app.AppWindow.Content.XamlRoot
                };
                await App.ShowDialogAsync(dialog);

                string uniqueTag = $"PurchaseReceipt_Edit_{_editingReceiptId.Value}";
                app.AppWindow.CloseTab(uniqueTag);
            }
        }
        else
        {
            var command = new CreatePurchaseReceiptCommand(
                SelectedSupplier.Id,
                ReceiptDate.DateTime,
                ReferenceNumber,
                globalDiscount,
                AdditionalChargesValue,
                Remarks,
                itemsDto);

            await mediator.Send(command);

            ReceiptItems.Clear();
            ReferenceNumber = string.Empty;
            TotalDiscount = "0";
            AdditionalCharges = "0";
            Remarks = string.Empty;
        }

        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    private void OnLineChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalAmount));
        OnPropertyChanged(nameof(TotalLineDiscounts));
        OnPropertyChanged(nameof(OverallTotalDiscount));
        OnPropertyChanged(nameof(NetAmount));
    }
    public async Task InitializeAsync(int? receiptId)
    {
        try
        {
            ErrorMessage = string.Empty;
        _editingReceiptId = receiptId;

        if (receiptId.HasValue)
        {
            SubmitButtonText = "Update Purchase Receipt";

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var receipt = await mediator.Send(new GetPurchaseReceiptByIdQuery(receiptId.Value));

            if (receipt != null)
            {
                SearchSupplierText = receipt.SupplierName;

                SelectedSupplier = new PartyDto(receipt.SupplierId, receipt.SupplierName, "", true, false);

                ReceiptDate = receipt.ReceiptDate;
                ReferenceNumber = receipt.ReferenceNumber;
                TotalDiscount = receipt.TotalDiscount.ToString("N0");
                AdditionalCharges = receipt.AdditionalCharges.ToString("N0");
                Remarks = receipt.Remarks;

                ReceiptItems.Clear();
                foreach (var item in receipt.Items)
                {
                    ReceiptItems.Add(new ReceiptItemUIModel
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        DiscountAmount = item.DiscountAmount
                    });
                }

                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(TotalLineDiscounts));
                OnPropertyChanged(nameof(NetAmount));
                OnPropertyChanged(nameof(OverallTotalDiscount));
            }
        }
        else
        {
            SubmitButtonText = "Submit Purchase Receipt";
        }

        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    #endregion
}
