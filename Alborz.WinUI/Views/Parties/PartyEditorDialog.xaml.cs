using Alborz.Application.Features.Parties.Queries;
using Microsoft.UI.Xaml.Controls;


namespace Alborz.WinUI.Views;

public sealed partial class PartyEditorDialog : ContentDialog
{
    public string PartyName => NameTextBox.Text;
    public string PartyPhone => PhoneTextBox.Text;
    public bool IsSupplier => SupplierCheckBox.IsChecked ?? false;
    public bool IsCustomer => CustomerCheckBox.IsChecked ?? false;

    public PartyEditorDialog(PartyDto? existingParty = null)
    {
        this.InitializeComponent();

        if (existingParty != null)
        {
            NameTextBox.Text = existingParty.Name;
            PhoneTextBox.Text = existingParty.Phone;
            SupplierCheckBox.IsChecked = existingParty.IsSupplier;
            CustomerCheckBox.IsChecked = existingParty.IsCustomer;
        }
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            args.Cancel = true;
        }
    }
}
