using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Alborz.WinUI.ViewModels;

namespace Alborz.WinUI.Views.Customers;

public sealed partial class CustomersPage : Page
{
    public CustomersViewModel ViewModel { get; }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string tag && tag == "CustomerRegistrationPage")
            Loaded += OpenRegistration;
    }

    private async void OpenRegistration(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Loaded -= OpenRegistration;
        await ViewModel.ShowAddDialogAsync();
    }

    public CustomersPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<CustomersViewModel>();
    }
}

