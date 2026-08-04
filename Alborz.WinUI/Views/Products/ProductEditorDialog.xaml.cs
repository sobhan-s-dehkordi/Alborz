using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls;
using Alborz.Application.Features.Products.Queries;

namespace Alborz.WinUI.Views.Products;

public sealed partial class ProductEditorDialog : ContentDialog
{

    #region <Properties>

    public bool IsEditMode { get; }
    public int EditProductId { get; }
    public string ProductName => NameTextBox.Text;
    public string Barcode => BarcodeTextBox.Text;
    public decimal PurchasePrice => decimal.TryParse(PurchasePriceTextBox.Text.Replace(",", ""), out var p) ? p : 0;
    public decimal SellPrice => decimal.TryParse(SellPriceTextBox.Text.Replace(",", ""), out var s) ? s : 0;
    public int InitialStock => int.TryParse(InitialStockTextBox.Text, out var i) ? i : 0;
    public int ReorderPoint => int.TryParse(ReorderPointTextBox.Text, out var r) ? r : 0;

    #endregion

    #region <Constructor>

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

    #endregion

    #region <Event Handlers>

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

    private void Currency_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        string rawText = sender.Text.Replace(",", "");

        if (string.IsNullOrWhiteSpace(rawText))
        {
            sender.Text = string.Empty;
            return;
        }

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
                sender.SelectionStart = Math.Max(0, newCursorPosition);
            }
        }
    }

    #endregion
}