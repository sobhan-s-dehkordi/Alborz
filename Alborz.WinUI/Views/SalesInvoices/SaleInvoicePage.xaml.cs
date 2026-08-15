using Alborz.Application.Features.Customers.Queries;
using Alborz.Application.Features.Products.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using ProjectName.WinUI.ViewModels;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Alborz.WinUI.Views.SalesInvoices;

public sealed partial class SaleInvoicePage : Page
{
    public SalesInvoiceViewModel ViewModel { get; }

    public SaleInvoicePage()
    {
        this.InitializeComponent();

        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<SalesInvoiceViewModel>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is int invoiceId)
        {
            _ = ViewModel.InitializeAsync(invoiceId);
        }
        else
        {
            _ = ViewModel.InitializeAsync(null);
        }
    }


    private void CustomerSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.SelectedCustomer = null;
                ViewModel.CustomerSearchResults.Clear();
                return;
            }

            ViewModel.SearchCustomersCommand.Execute(sender.Text);
        }
    }

    private void CustomerSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is CustomerDto selectedCustomer)
        {
            ViewModel.SelectedCustomer = selectedCustomer;
            sender.Text = selectedCustomer.Name;
        }
    }


    private void ProductSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.SelectProduct(null);
                ViewModel.ProductSearchResults.Clear();
                return;
            }

            ViewModel.SearchProductsCommand.Execute(sender.Text);
        }
    }

    private void ProductSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is ProductDto selectedProduct)
        {
            ViewModel.SelectProduct(selectedProduct);
        }
    }


    private void RemoveItemButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is InvoiceItemUIModel item)
        {
            ViewModel.RemoveItemCommand.Execute(item);
        }
    }

    private void Currency_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        string rawText = sender.Text.Replace(",", "");

        if (string.IsNullOrWhiteSpace(rawText)) return;

        if (rawText.Any(c => !char.IsDigit(c)))
        {
            rawText = new string(rawText.Where(char.IsDigit).ToArray());
        }

        if (decimal.TryParse(rawText, out decimal value))
        {
            string formattedText = value.ToString("N0");

            if (sender.Text != formattedText)
            {
                int cursorPositionFromEnd = sender.Text.Length - sender.SelectionStart;
                sender.Text = formattedText;
                int newCursorPosition = formattedText.Length - cursorPositionFromEnd;
                sender.SelectionStart = System.Math.Max(0, newCursorPosition);
            }
        }
    }
}

