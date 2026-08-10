using Alborz.Application.Features.Parties.Queries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using ProjectName.WinUI.ViewModels;

namespace Alborz.WinUI.Views.PurchaseReceipts;

public sealed partial class PurchaseHistoryPage : Page
{
    #region <Properties>

    public PurchaseHistoryViewModel ViewModel { get; }

    #endregion

    #region <Constructor>

    public PurchaseHistoryPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<PurchaseHistoryViewModel>();
    }

    #endregion

    #region <Event Handlers>

    private void SupplierSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.SelectedSupplier = null;
                ViewModel.SupplierSearchResults.Clear();
                return;
            }

            ViewModel.SearchSuppliersCommand.Execute(sender.Text);
        }
    }

    private void SupplierSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is PartyDto selectedSupplier)
        {
            ViewModel.SelectedSupplier = selectedSupplier;
            sender.Text = selectedSupplier.Name;
        }
    }

    #endregion
}
