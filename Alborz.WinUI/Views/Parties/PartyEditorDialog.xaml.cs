using Microsoft.UI.Xaml.Controls;
using Alborz.Application.Features.Parties.Queries;

namespace Alborz.WinUI.Views.Parties;

public sealed partial class PartyEditorDialog : ContentDialog
{

    #region <Properties>

    public string PartyName => NameTextBox.Text;
    public string PartyPhone => PhoneTextBox.Text;
    public bool IsSupplier => SupplierCheckBox.IsChecked ?? false;
    public bool IsCustomer => CustomerCheckBox.IsChecked ?? false;

    #endregion

    #region <Constructor>

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

    #endregion

    #region <Event Handlers>

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            args.Cancel = true;
        }
    }

    #endregion
}