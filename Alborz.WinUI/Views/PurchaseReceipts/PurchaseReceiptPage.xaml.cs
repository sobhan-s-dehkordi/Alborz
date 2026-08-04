using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Alborz.Application.Features.Parties.Queries;
using Alborz.Application.Features.Products.Queries;
using ProjectName.WinUI.ViewModels;

namespace Alborz.WinUI.Views.PurchaseReceipts;

public sealed partial class PurchaseReceiptPage : Page
{

    #region <Properties>

    public PurchaseReceiptViewModel ViewModel { get; }

    #endregion

    #region <Constructor>

    public PurchaseReceiptPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<PurchaseReceiptViewModel>();
    }

    #endregion

    #region <Event Handlers>

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

    private void SupplierSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.SelectedSupplier = null;
                ViewModel.SupplierSearchResults.Clear();
                return;
            }

            ViewModel.SearchSuppliersCommand.Execute(sender.Text);
        }
    }

    private void SupplierSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is PartyDto selectedSupplier)
        {
            ViewModel.SelectedSupplier = selectedSupplier;
            sender.Text = selectedSupplier.Name;
        }
    }

    private void RemoveItemButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ReceiptItemUIModel item)
        {
            ViewModel.RemoveItemCommand.Execute(item);
        }
    }

    private void NumericOnly_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (sender.Text.Any(c => !char.IsDigit(c)))
        {
            string numericText = new string(sender.Text.Where(char.IsDigit).ToArray());

            int selectionStart = sender.SelectionStart;
            sender.Text = numericText;

            sender.SelectionStart = selectionStart > sender.Text.Length ? sender.Text.Length : selectionStart;
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

    #endregion

}