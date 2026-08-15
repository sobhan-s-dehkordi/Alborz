using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectName.WinUI.ViewModels;

public partial class InvoiceItemUIModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private int _quantity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private decimal _discountAmount;

    public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;
}