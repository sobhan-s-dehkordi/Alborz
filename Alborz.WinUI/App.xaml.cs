using Alborz.Application;
using Alborz.Infrastructure;
using Alborz.Infrastructure.Data;
using Alborz.WinUI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Alborz.WinUI;

public partial class App : Microsoft.UI.Xaml.Application
{
    public MainWindow AppWindow { get; private set; } = null!;
    public IServiceProvider Services { get; }

    public App()
    {
        UnhandledException += (_, args) => Diagnostics.StartupDiagnostics.Record(args.Exception);
        try
        {
            InitializeComponent();
            Services = ConfigureServices();
        }
        catch (Exception ex)
        {
            Diagnostics.StartupDiagnostics.Record(ex);
            throw;
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<Alborz.WinUI.Services.AppearanceService>();
        services.AddApplication();
        services.AddInfrastructure(DatabaseSettings.ConnectionString);
        services.AddTransient<PurchaseReceiptViewModel>();
        services.AddTransient<PurchaseHistoryViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<PartiesViewModel>();
        services.AddTransient<SalesInvoiceViewModel>();
        services.AddTransient<SalesHistoryViewModel>();
        services.AddTransient<CustomersViewModel>();
        services.AddTransient<ReportsViewModel>();
        services.AddTransient<Alborz.WinUI.ViewModels.Settings.SettingsViewModel>();
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }

    public static async System.Threading.Tasks.Task<Microsoft.UI.Xaml.Controls.ContentDialogResult> ShowDialogAsync(
        Microsoft.UI.Xaml.Controls.ContentDialog dialog)
    {
        ((App)Current).Services.GetRequiredService<Alborz.WinUI.Services.AppearanceService>().ApplyTo(dialog);
        return await dialog.ShowAsync();
    }

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        try
        {
            var appearance = Services.GetRequiredService<Alborz.WinUI.Services.AppearanceService>();
            Resources["Appearance"] = appearance.State;
        }
        catch (Exception ex)
        {
            Diagnostics.StartupDiagnostics.Record(ex);
            throw;
        }
        AppWindow = new MainWindow();
        AppWindow.SetDatabaseReady(false, "Checking SQL Server connection�");
        AppWindow.Activate();
        try
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var ready = await db.Database.CanConnectAsync()
                && !System.Linq.Enumerable.Any(await db.Database.GetPendingMigrationsAsync());
            AppWindow.SetDatabaseReady(ready, ready ? string.Empty :
                "Database is unavailable or requires migration. Check the connection and apply migrations before using business features. Appearance settings remain available.");
        }
        catch (Exception)
        {
            AppWindow.SetDatabaseReady(false,
                "Database connection failed. Check SQL Server and ALBORZ_SQLSERVER_CONNECTION. Appearance settings remain available.");
        }
    }
}
