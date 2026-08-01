using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using ProjectName.WinUI.ViewModels;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Alborz.WinUI.Views;

public sealed partial class PurchaseReceiptPage : Page
{
    public PurchaseReceiptViewModel ViewModel { get; }

    public PurchaseReceiptPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<PurchaseReceiptViewModel>();
    }

    // Trigger search when user presses Enter in the Barcode box
    private void Barcode_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ViewModel.FindProductCommand.Execute(null);
        }
    }

    private void NumericOnly_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (sender.Text.Any(c => !char.IsDigit(c)))
        {
            int pos = sender.SelectionStart;
            sender.Text = new string(sender.Text.Where(char.IsDigit).ToArray());
            sender.SelectionStart = pos > sender.Text.Length ? sender.Text.Length : pos;
        }
    }

    private void Currency_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        string rawText = sender.Text.Replace(",", "");
        if (string.IsNullOrWhiteSpace(rawText)) return;
        if (rawText.Any(c => !char.IsDigit(c))) rawText = new string(rawText.Where(char.IsDigit).ToArray());

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
