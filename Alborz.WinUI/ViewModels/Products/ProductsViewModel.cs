using Alborz.Application.Features.Products.Commands;
using Alborz.Application.Features.Products.Queries;
using MediatR;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Alborz.WinUI.ViewModels.Products;

public partial class ProductsViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    public ObservableCollection<ProductDto> Products { get; } = new();

    [ObservableProperty]
    private string _searchTerm = string.Empty;

    [ObservableProperty] private string _productName = string.Empty;
    [ObservableProperty] private string _barcode = string.Empty;
    [ObservableProperty] private decimal _purchasePrice;
    [ObservableProperty] private decimal _sellPrice;
    [ObservableProperty] private int _stockQuantity;
    [ObservableProperty] private int _reorderPoint;

    [ObservableProperty] private string _errorMessage = string.Empty;

    public ProductsViewModel(IMediator mediator)
    {
        _mediator = mediator;
        _ = LoadProductsAsync();
    }

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            var query = new GetProductsQuery(SearchTerm);
            var result = await _mediator.Send(query);

            Products.Clear();
            foreach (var item in result)
            {
                Products.Add(item);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load products: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task AddProductAsync()
    {
        try
        {
            ErrorMessage = string.Empty;
            var command = new CreateProductCommand(
                ProductName,
                Barcode,
                PurchasePrice,
                SellPrice,
                StockQuantity,
                ReorderPoint);

            await _mediator.Send(command);

            ProductName = string.Empty;
            Barcode = string.Empty;
            PurchasePrice = 0;
            SellPrice = 0;
            StockQuantity = 0;
            ReorderPoint = 0;

            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
