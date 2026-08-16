using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Alborz.Application.Features.Customers.Queries;
using Alborz.WinUI.ViewModels;

namespace Alborz.WinUI.Views.SalesInvoices;

public sealed partial class SalesHistoryPage : Page
{
    public SalesHistoryViewModel ViewModel { get; }

    public SalesHistoryPage()
    {
        this.InitializeComponent();

        // دریافت ViewModel از کانتینر DI
        ViewModel = ((App)Microsoft.UI.Xaml.Application.Current).Services.GetRequiredService<SalesHistoryViewModel>();
    }

    // ==========================================
    // رویدادهای جستجوی مشتری
    // ==========================================
    private void CustomerSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // فقط اگر خود کاربر متنی تایپ کرده باشد جستجو انجام می‌شود
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.SelectedCustomer = null;
                ViewModel.CustomerSearchResults.Clear();
                return;
            }

            ViewModel.SearchCustomersCommand.Execute(sender.Text);
        }
    }

    private void CustomerSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        // وقتی کاربر یکی از موارد لیست پیشنهادی را انتخاب می‌کند
        if (args.SelectedItem is CustomerDto selectedCustomer)
        {
            ViewModel.SelectedCustomer = selectedCustomer;
            sender.Text = selectedCustomer.Name;
        }
    }
}