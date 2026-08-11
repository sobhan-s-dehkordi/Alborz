using Alborz.Application.Features.PurchaseReceipts.Commands;
using Microsoft.UI.Xaml.Controls;


namespace Alborz.WinUI.Views.PurchaseReceipts;

public sealed partial class ReceiptDetailsDialog : ContentDialog
{
    #region <Properties>

    public PurchaseReceiptDto Receipt { get; }

    #endregion

    #region <Constructor>

    public ReceiptDetailsDialog(PurchaseReceiptDto receipt)
    {
        this.InitializeComponent();
        Receipt = receipt;
    }

    #endregion
}
