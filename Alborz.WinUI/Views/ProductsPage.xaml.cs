using Alborz.WinUI.ViewModels.Products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Alborz.WinUI.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ProductsPage : Page
{
    public ProductsViewModel ViewModel { get; }
    public ProductsPage()
    {
        InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ProductsViewModel>();
    }
}
