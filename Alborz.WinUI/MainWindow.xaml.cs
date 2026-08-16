using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Alborz.WinUI;

public sealed partial class MainWindow : Window
{

    #region <Constructor>

    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region <Methods>

    public void OpenOrFocusTab(string header, Type pageType, IconElement menuIcon, string uniqueTag, object parameter = null)
    {
        var existingTab = MainTabView.TabItems
            .OfType<TabViewItem>()
            .FirstOrDefault(t => t.Tag?.ToString() == uniqueTag);

        if (existingTab != null)
        {
            MainTabView.SelectedItem = existingTab;
            return;
        }

        var frame = new Frame();

        frame.Navigate(pageType, parameter);

        var newTab = new TabViewItem
        {
            Header = header,
            Content = frame,
            Tag = uniqueTag
        };

        if (menuIcon is FontIcon fontIcon)
        {
            newTab.IconSource = new FontIconSource { Glyph = fontIcon.Glyph };
        }

        MainTabView.TabItems.Add(newTab);
        MainTabView.SelectedItem = newTab;
    }

    public void CloseTab(string uniqueTag)
    {
        var tabToClose = MainTabView.TabItems
            .OfType<TabViewItem>()
            .FirstOrDefault(t => t.Tag?.ToString() == uniqueTag);

        if (tabToClose != null)
        {
            MainTabView.TabItems.Remove(tabToClose);
        }
    }

    #endregion

    #region <Event Handlers>

    private void MainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItemContainer as NavigationViewItem;

        if (item == null || item.MenuItems.Count > 0 || item.Tag == null)
        {
            return;
        }

        string pageTag = item.Tag.ToString();
        string header = item.Content.ToString();

        Type pageType = pageTag switch
        {
            "ProductsPage" => typeof(Views.Products.ProductsPage),
            "PurchaseReceiptPage" => typeof(Views.PurchaseReceipts.PurchaseReceiptPage),
            "PartiesPage" => typeof(Views.Parties.PartiesPage),
            "PurchaseHistoryPage" => typeof(Views.PurchaseReceipts.PurchaseHistoryPage),
            "SaleInvoicePage" => typeof(Views.SalesInvoices.SaleInvoicePage),
            "InvoiceArchivePage" => typeof(Views.SalesInvoices.SalesHistoryPage),
            "CustomersPage" => typeof(Views.Customers.CustomersPage)

        };

        if (pageType != null)
        {
            OpenOrFocusTab(header, pageType, item.Icon, pageTag);
        }
    }

    private void MainTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }

    #endregion
}