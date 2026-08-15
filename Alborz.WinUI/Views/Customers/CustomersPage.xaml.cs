using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using ProjectName.WinUI.ViewModels;

namespace Alborz.WinUI.Views.Customers;

public sealed partial class CustomersPage : Page
{
    public CustomersViewModel ViewModel { get; }

    public CustomersPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<CustomersViewModel>();
    }
}