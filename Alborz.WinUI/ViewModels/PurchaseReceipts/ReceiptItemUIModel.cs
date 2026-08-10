using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace ProjectName.WinUI.ViewModels;

public partial class ReceiptItemUIModel : ObservableObject
{

    #region <Properties>

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    #endregion

    #region <Observable & Computed Properties>

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private int _quantity;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private decimal _unitPrice;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private decimal _discountAmount;

    public decimal TotalPrice => (Quantity * UnitPrice) - DiscountAmount;

    #endregion

}