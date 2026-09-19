using CommunityToolkit.Mvvm.ComponentModel;

namespace Alborz.WinUI.ViewModels.SalesInvoices;

public partial class InvoiceItemUIModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    public partial decimal UnitPrice { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    public partial int Quantity { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    public partial decimal DiscountAmount { get; set; }

    public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;
}


