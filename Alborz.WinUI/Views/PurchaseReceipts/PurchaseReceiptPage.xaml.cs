using Alborz.Application.Features.Products.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using ProjectName.WinUI.ViewModels;
using System.Linq;

namespace Alborz.WinUI.Views
{
    public sealed partial class PurchaseReceiptPage : Page
    {
        public PurchaseReceiptViewModel ViewModel { get; }

        public PurchaseReceiptPage()
        {
            this.InitializeComponent();
            ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<PurchaseReceiptViewModel>();
        }

        
        private void ProductSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
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

    
        private void Currency_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            string rawText = sender.Text.Replace(",", "");
            if (string.IsNullOrWhiteSpace(rawText)) return;

            if (rawText.Any(c => !char.IsDigit(c)))
                rawText = new string(rawText.Where(char.IsDigit).ToArray());

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
}