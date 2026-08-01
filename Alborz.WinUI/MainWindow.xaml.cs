using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;


namespace Alborz.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void MainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var item = args.InvokedItemContainer as NavigationViewItem;

        if (item == null || item.MenuItems.Count > 0 || item.Tag == null)
            return;

        string pageTag = item.Tag.ToString();
        string header = item.Content.ToString();

        Type pageType = pageTag switch
        {
            "ProductsPage" => typeof(Views.ProductsPage),
            "PurchaseReceiptPage" => typeof(Views.PurchaseReceiptPage)
        };

        OpenOrFocusTab(header, pageType, item.Icon);
    }

    private void OpenOrFocusTab(string header, Type pageType, IconElement menuIcon)
    {
        var existingTab = MainTabView.TabItems
            .OfType<TabViewItem>()
            .FirstOrDefault(t => t.Header.ToString() == header);

        if (existingTab != null)
        {
            MainTabView.SelectedItem = existingTab;
            return;
        }

        var frame = new Frame();
        frame.Navigate(pageType);

        var newTab = new TabViewItem
        {
            Header = header,
            Content = frame
        };

        if (menuIcon is FontIcon fontIcon)
        {
            newTab.IconSource = new FontIconSource { Glyph = fontIcon.Glyph };
        }

        MainTabView.TabItems.Add(newTab);
        MainTabView.SelectedItem = newTab;
    }

    private void MainTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }
}
