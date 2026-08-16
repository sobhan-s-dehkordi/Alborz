using Alborz.Application.Features.Invoices.Queries;
using Microsoft.UI.Xaml.Controls;

namespace Alborz.WinUI.Views.SalesInvoices;

public sealed partial class InvoiceDetailsDialog : ContentDialog
{
    // این پراپرتی در XAML برای x:Bind استفاده می‌شود
    public SalesInvoiceDetailDto Invoice { get; }

    public InvoiceDetailsDialog(SalesInvoiceDetailDto invoice)
    {
        // مقداردهی فاکتور قبل از اجرای InitializeComponent انجام می‌شود
        Invoice = invoice;

        this.InitializeComponent();
    }
}