using Alborz.Application.Features.Products.Queries;
using Alborz.Application.Features.PurchaseReceipts.Commands;
using Alborz.Application.Features.PurchaseReceipts.Queries;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectName.WinUI.ViewModels;

public partial class ReceiptItemUIModel : ObservableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

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
}
