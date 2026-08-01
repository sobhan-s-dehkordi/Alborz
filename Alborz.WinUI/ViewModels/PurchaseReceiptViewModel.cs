using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Application.Features.PurchaseReceipts.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectName.WinUI.ViewModels;

public partial class PurchaseReceiptViewModel : ObservableObject
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ObservableCollection<ReceiptItemUIModel> ReceiptItems { get; } = new();

    [ObservableProperty] private string _supplierName = string.Empty;
    [ObservableProperty] private string _scannedBarcode = string.Empty;

    [ObservableProperty] private string _statusMessage = string.Empty;

    [ObservableProperty] private int _inputQuantity = 1;
    [ObservableProperty] private string _inputPrice = "0";

    private int _currentProductId;
    [ObservableProperty] private string _currentProductName = "Scan a product...";

    public decimal TotalReceiptAmount => ReceiptItems.Sum(x => x.TotalPrice);

    public PurchaseReceiptViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        ReceiptItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalReceiptAmount));
    }

    [RelayCommand]
    public async Task FindProductAsync()
    {
        if (string.IsNullOrWhiteSpace(ScannedBarcode)) return;

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var product = await mediator.Send(new GetProductByBarcodeQuery(ScannedBarcode));

        if (product != null)
        {
            _currentProductId = product.Id;
            CurrentProductName = product.Name;
            InputPrice = product.PurchasePrice.ToString("N0"); // قیمت پیش‌فرض خرید قبلی
            InputQuantity = 1;
            StatusMessage = string.Empty;
        }
        else
        {
            StatusMessage = "Product not found!";
            _currentProductId = 0;
            CurrentProductName = "Unknown";
        }
    }

    [RelayCommand]
    public void AddToReceipt()
    {
        if (_currentProductId == 0) return;

        decimal price = decimal.TryParse(InputPrice.Replace(",", ""), out var p) ? p : 0;

        var existingItem = ReceiptItems.FirstOrDefault(x => x.ProductId == _currentProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += InputQuantity;
            existingItem.UnitPrice = price; // Update to latest entered price
        }
        else
        {
            ReceiptItems.Add(new ReceiptItemUIModel
            {
                ProductId = _currentProductId,
                ProductName = CurrentProductName,
                Quantity = InputQuantity,
                UnitPrice = price
            });
        }

        ScannedBarcode = string.Empty;
        CurrentProductName = "Scan a product...";
        InputQuantity = 1;
        InputPrice = "0";
        _currentProductId = 0;

        OnPropertyChanged(nameof(TotalReceiptAmount));
    }

    [RelayCommand]
    public async Task SaveReceiptAsync()
    {
        if (!ReceiptItems.Any()) return;

        var itemsDto = ReceiptItems.Select(x => new PurchaseItemDto(x.ProductId, x.Quantity, x.UnitPrice)).ToList();
        var command = new CreatePurchaseReceiptCommand(SupplierName, itemsDto);

        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(command);

        // پاک کردن فرم پس از ثبت موفق
        ReceiptItems.Clear();
        SupplierName = string.Empty;
        StatusMessage = "Receipt saved successfully! Stock updated.";
    }
}