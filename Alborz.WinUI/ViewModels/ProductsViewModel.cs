using Alborz.Application.Features.Products.Commands;
using Alborz.Application.Features.Products.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProjectName.WinUI.ViewModels
{
    public partial class ProductsViewModel : ObservableObject
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public ObservableCollection<ProductDto> Products { get; } = new();

        [ObservableProperty] private string _searchCodeFrom = string.Empty;
        [ObservableProperty] private string _searchCodeTo = string.Empty;
        [ObservableProperty] private string _searchBarcode = string.Empty;
        [ObservableProperty] private string _searchName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsProductSelected))]
        private ProductDto? _selectedProduct;
        public bool IsProductSelected => SelectedProduct != null;

        public ProductsViewModel(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _ = LoadProductsAsync();
        }

        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            int? codeFrom = int.TryParse(SearchCodeFrom, out int f) ? f : null;
            int? codeTo = int.TryParse(SearchCodeTo, out int t) ? t : null;

            var query = new GetProductsQuery(codeFrom, codeTo, SearchBarcode, SearchName);

            // Create a manual scope for the Read operation
            using (var scope = _scopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var result = await mediator.Send(query);

                Products.Clear();

                if (result is not null)
                    foreach (var item in result) Products.Add(item);
                
                SelectedProduct = null;
            }
        }

        public async Task ProcessCreateAsync(string name, string barcode, decimal buyPrice, decimal sellPrice, int stock, int reorder)
        {
            var command = new CreateProductCommand(name, barcode, buyPrice, sellPrice, stock, reorder);

            // Create a manual scope for the Write operation
            using (var scope = _scopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(command);
            }
            // Once the using block closes, the transaction is complete and the DbContext is destroyed.

            await LoadProductsAsync();
        }

        // Method to process the update
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
    }
}