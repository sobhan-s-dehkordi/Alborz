using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Alborz.Application.Features.Products.Commands;
using Alborz.Application.Features.Products.Queries;

namespace ProjectName.WinUI.ViewModels;

public partial class ProductsViewModel : ObservableObject
{

    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region <Collections>

    public ObservableCollection<ProductDto> Products { get; } = new();

    #endregion

    #region <Observable Properties>

    [ObservableProperty]
    private string _searchCodeFrom = string.Empty;

    [ObservableProperty]
    private string _searchCodeTo = string.Empty;

    [ObservableProperty]
    private string _searchBarcode = string.Empty;

    [ObservableProperty]
    private string _searchName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProductSelected))]
    private ProductDto? _selectedProduct;

    public bool IsProductSelected => SelectedProduct != null;

    #endregion

    #region <Constructor>

    public ProductsViewModel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _ = LoadProductsAsync();
    }

    #endregion

    #region <Commands & Methods>

    [RelayCommand]
    public async Task LoadProductsAsync()
    {
        int? codeFrom = int.TryParse(SearchCodeFrom, out int f) ? f : null;
        int? codeTo = int.TryParse(SearchCodeTo, out int t) ? t : null;

        var query = new GetProductsQuery(codeFrom, codeTo, SearchBarcode, SearchName);

        using (var scope = _scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var result = await mediator.Send(query);

            Products.Clear();

            if (result is not null)
            {
                foreach (var item in result)
                {
                    Products.Add(item);
                }
            }

            SelectedProduct = null;
        }
    }

    public async Task ProcessCreateAsync(string name, string barcode, decimal buyPrice, decimal sellPrice, int stock, int reorder)
    {
        var command = new CreateProductCommand(name, barcode, buyPrice, sellPrice, stock, reorder);

        using (var scope = _scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(command);
        }

        await LoadProductsAsync();
    }

    public async Task ProcessUpdateAsync(int id, string name, string barcode, decimal buyPrice, decimal sellPrice, int reorder)
    {
        var command = new UpdateProductCommand(id, name, barcode, buyPrice, sellPrice, reorder);

        using (var scope = _scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(command);
        }

        await LoadProductsAsync();
    }

    #endregion
}