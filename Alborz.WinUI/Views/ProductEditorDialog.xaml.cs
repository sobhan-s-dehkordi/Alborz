using Alborz.Application.Features.Products.Queries;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Alborz.WinUI.Views;


public sealed partial class ProductEditorDialog : ContentDialog
{
    public bool IsEditMode { get; }
    public int EditProductId { get; }

    public ProductEditorDialog()
    {
        this.InitializeComponent();
        IsEditMode = false;
        Title = "Create New Product";
    }

    public ProductEditorDialog(ProductDto productToEdit)
    {
        this.InitializeComponent();
        IsEditMode = true;
        EditProductId = productToEdit.Id;
        Title = $"Edit Product: {productToEdit.Name}";

        NameTextBox.Text = productToEdit.Name;
        BarcodeTextBox.Text = productToEdit.Barcode;
        PurchasePriceTextBox.Text = productToEdit.PurchasePrice.ToString("G");
        SellPriceTextBox.Text = productToEdit.SellPrice.ToString("G");

        InitialStockTextBox.Text = productToEdit.StockQuantity.ToString();
        InitialStockTextBox.IsEnabled = false;
    }

    private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
    {
        NameTextBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
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

    private void DecimalOnly_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        string text = sender.Text;
        if (text.Any(c => !char.IsDigit(c) && c != '.'))
        {
            int pos = sender.SelectionStart;
            sender.Text = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());
            sender.SelectionStart = pos > sender.Text.Length ? sender.Text.Length : pos;
        }
    }

    public string ProductName => NameTextBox.Text;
    public string Barcode => BarcodeTextBox.Text;
    public decimal PurchasePrice => decimal.TryParse(PurchasePriceTextBox.Text, out var p) ? p : 0;
    public decimal SellPrice => decimal.TryParse(SellPriceTextBox.Text, out var s) ? s : 0;
    public int InitialStock => int.TryParse(InitialStockTextBox.Text, out var i) ? i : 0;
    public int ReorderPoint => int.TryParse(ReorderPointTextBox.Text, out var r) ? r : 0;
}
