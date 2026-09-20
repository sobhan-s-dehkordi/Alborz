using Microsoft.Extensions.DependencyInjection;
using Alborz.Application.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Globalization;

namespace Alborz.WinUI.Views.Common;

public sealed class DocumentLineDialog : ContentDialog
{
    public DocumentLine? Result { get; private set; }
    public DocumentLineDialog(int productId, string name, int quantity, decimal price, decimal discount)
    {
        Title = $"Edit — {name}";
        PrimaryButtonText = "Save row";
        CloseButtonText = "Cancel";
        DefaultButton = ContentDialogButton.Primary;
        var quantityInput = new TextBox { Header = "Quantity", Text = quantity.ToString(CultureInfo.CurrentCulture) };
        var priceInput = new TextBox { Header = "Unit price", Text = price.ToString(CultureInfo.CurrentCulture) };
        var discountInput = new TextBox { Header = "Total row discount", Text = discount.ToString(CultureInfo.CurrentCulture) };
        var error = new TextBlock { TextWrapping = TextWrapping.Wrap };
        var panel = new StackPanel { Spacing = 12, MinWidth = 320 };
        panel.Children.Add(quantityInput);
        panel.Children.Add(priceInput);
        panel.Children.Add(discountInput);
        panel.Children.Add(error);
        Content = panel;
        var appearance = ((App)Microsoft.UI.Xaml.Application.Current).Services
            .GetRequiredService<Alborz.WinUI.Services.AppearanceService>();
        appearance.ApplyTo(quantityInput);
        appearance.ApplyTo(priceInput);
        appearance.ApplyTo(discountInput);
        PrimaryButtonClick += (_, args) =>
        {
            try
            {
                if (!int.TryParse(quantityInput.Text, out var q) ||
                    !decimal.TryParse(priceInput.Text, out var p) ||
                    !decimal.TryParse(discountInput.Text, out var d))
                    throw new ArgumentException("Enter valid numeric values; quantity must be a whole number.");
                Result = DocumentLineEditor.Validate(productId, q, p, d);
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException)
            {
                args.Cancel = true;
                error.Text = ex.Message;
            }
        };
    }
}

