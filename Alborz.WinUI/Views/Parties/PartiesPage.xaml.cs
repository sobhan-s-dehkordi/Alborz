using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Alborz.Application.Features.Parties.Queries;
using ProjectName.WinUI.ViewModels;

namespace Alborz.WinUI.Views.Parties;

public sealed partial class PartiesPage : Page
{

    #region <Properties>

    public PartiesViewModel ViewModel { get; }

    #endregion

    #region <Constructor>

    public PartiesPage()
    {
        this.InitializeComponent();
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<PartiesViewModel>();
    }

    #endregion

    #region <Methods>

    public bool HasSelectedParty(PartyDto? party) => party != null;

    #endregion

    #region <Event Handlers>

    private void SearchBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ViewModel.LoadPartiesCommand.Execute(null);
        }
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new PartyEditorDialog();
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.AddPartyAsync(dialog.PartyName, dialog.PartyPhone, dialog.IsSupplier, dialog.IsCustomer);
        }
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedParty == null) return;

        var dialog = new PartyEditorDialog(ViewModel.SelectedParty);
        dialog.XamlRoot = this.XamlRoot;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.UpdatePartyAsync(
                ViewModel.SelectedParty.Id,
                dialog.PartyName,
                dialog.PartyPhone,
                dialog.IsSupplier,
                dialog.IsCustomer);
        }
    }

    #endregion

}