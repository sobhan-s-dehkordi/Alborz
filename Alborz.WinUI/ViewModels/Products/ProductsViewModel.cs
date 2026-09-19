using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Alborz.Application.Features.Products.Commands;
using Alborz.Application.Features.Products.Queries;

namespace Alborz.WinUI.ViewModels.Products;

public partial class ProductsViewModel : Alborz.WinUI.ViewModels.Common.ViewModelBase
{

    #region <Fields>

    private readonly IServiceScopeFactory _scopeFactory;

    #endregion

    #region <Collections>

    public ObservableCollection<ProductDto> Products { get; } = new();

    #endregion

    #region <Observable Properties>

    [ObservableProperty]
    public partial string SearchCodeFrom { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchCodeTo { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchBarcode { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchName { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsProductSelected))]
    public partial ProductDto? SelectedProduct { get; set; }

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
        try
        {
            ErrorMessage = string.Empty;
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
        catch (System.Exception ex) { ReportError(ex); }
    }

    public async Task ProcessCreateAsync(string name, string barcode, decimal buyPrice, decimal sellPrice, int stock, int reorder)
    {
        try
        {
            ErrorMessage = string.Empty;
        var command = new CreateProductCommand(name, barcode, buyPrice, sellPrice, stock, reorder);

        using (var scope = _scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(command);
        }

        await LoadProductsAsync();
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    public async Task ProcessUpdateAsync(int id, string name, string barcode, decimal buyPrice, decimal sellPrice, int reorder)
    {
        try
        {
            ErrorMessage = string.Empty;
        var command = new UpdateProductCommand(id, name, barcode, buyPrice, sellPrice, reorder);

        using (var scope = _scopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(command);
        }

        await LoadProductsAsync();
    
        }
        catch (System.Exception ex) { ReportError(ex); }
    }

    #endregion
}


