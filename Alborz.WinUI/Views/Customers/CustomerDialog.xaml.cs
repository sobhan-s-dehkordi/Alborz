using Microsoft.UI.Xaml.Controls;

namespace Alborz.WinUI.Views.Customers;

public sealed partial class CustomerDialog : ContentDialog
{
    public string DialogTitle { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalCode { get; set; } = string.Empty;

    public int? EditingCustomerId { get; }

    public CustomerDialog(Application.Features.Customers.Queries.CustomerDto? existingCustomer = null)
    {
        this.InitializeComponent();

        if (existingCustomer != null)
        {
            DialogTitle = "Edit Customer";
            EditingCustomerId = existingCustomer.Id;
            CustomerName = existingCustomer.Name;
            PhoneNumber = existingCustomer.PhoneNumber;
            NationalCode = existingCustomer.NationalCode;
        }
        else
        {
            DialogTitle = "Add New Customer";
            EditingCustomerId = null;
        }
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(PhoneNumber))
        {
            args.Cancel = true;
            ErrorInfoBar.Message = "Name and Phone Number are required!";
            ErrorInfoBar.IsOpen = true;
        }
    }
}
