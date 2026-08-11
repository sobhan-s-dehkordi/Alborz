using Alborz.Application.Contracts;
using Alborz.Application.Features.Products.Commands;
using Alborz.Infrastructure.Data;
using Alborz.Infrastructure.Repositories;
using Alborz.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ProjectName.WinUI.ViewModels;
using System;
using System.IO;

namespace Alborz.WinUI;

public partial class App : Microsoft.UI.Xaml.Application
{

    #region <Fields>

    public MainWindow AppWindow { get; private set; }

    #endregion

    #region <Properties>

    public IServiceProvider Services { get; }

    #endregion

    #region <Constructor>

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();

        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    #endregion

    #region <Methods>

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = Path.Combine(localAppData, "AlborzApp");

        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        string dbPath = Path.Combine(appFolder, "Alborz.db");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories & UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();

        // Services
        services.AddScoped<IExcelExportService, ExcelExportService>();

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ProductCommandHandlers).Assembly));

        // ViewModels
        services.AddTransient<PurchaseReceiptViewModel>();
        services.AddTransient<PurchaseHistoryViewModel>();
        services.AddTransient<ProductsViewModel>();
        services.AddTransient<PartiesViewModel>();

        return services.BuildServiceProvider();
    }

    #endregion

    #region <Overrides>

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        AppWindow = new MainWindow();
        AppWindow.Activate();
    }

    #endregion

}