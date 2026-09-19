using Alborz.Application.Features.Reports.Queries;
using Alborz.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Alborz.WinUI.Views.Reports;

public sealed partial class ReportsPage : Page
{
    public ReportsViewModel ViewModel { get; }
    public ReportsPage()
    {
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ReportsViewModel>();
        InitializeComponent();
    }
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.Kind = e.Parameter switch
        {
            "TopProductsPage" => ReportKind.TopProducts,
            "TopCustomersPage" => ReportKind.TopCustomers,
            _ => ReportKind.InventoryLedger
        };
        Heading.Text = ViewModel.Kind switch
        {
            ReportKind.TopProducts => "Top 100 products — sales after line discounts",
            ReportKind.TopCustomers => "Top 100 customers — final invoice amounts",
            _ => "Inventory ledger — current documents"
        };
        ProductFilter.Visibility = ViewModel.Kind == ReportKind.InventoryLedger ? Visibility.Visible : Visibility.Collapsed;
        if (ViewModel.Kind != ReportKind.InventoryLedger) await ViewModel.LoadAsync();
    }
}
