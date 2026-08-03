using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ProjectName.WinUI.ViewModels;
using System;
using System.Linq;

namespace Alborz.WinUI.Views;


public sealed partial class ProductsPage : Page
{
    public ProductsViewModel ViewModel { get; }
    public ProductsPage()
    {
        InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<ProductsViewModel>();
    }

    private async void CreateProduct_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ProductEditorDialog();

        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            try
            {
                await ViewModel.ProcessCreateAsync(
                    dialog.ProductName,
                    dialog.Barcode,
                    dialog.PurchasePrice,
                    dialog.SellPrice,
                    dialog.InitialStock,
                    dialog.ReorderPoint
                );
            }
            catch (Exception ex)
            {
                await ShowErrorDialogAsync(ex.Message);
            }
        }
    }

    private async void EditProduct_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedProduct == null) return;

        var dialog = new ProductEditorDialog(ViewModel.SelectedProduct);
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            try
            {
                await ViewModel.ProcessUpdateAsync(
                    dialog.EditProductId,
                    dialog.ProductName,
                    dialog.Barcode,
                    dialog.PurchasePrice,
                    dialog.SellPrice,
                    dialog.ReorderPoint
                );
            }
            catch (Exception ex)
            {
                await ShowErrorDialogAsync(ex.Message);
            }
        }
    }

    private async System.Threading.Tasks.Task ShowErrorDialogAsync(string errorMessage)
    {
        var errorDialog = new ContentDialog
        {
            Title = "Error",
            Content = errorMessage,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await errorDialog.ShowAsync();
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
}
