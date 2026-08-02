using Alborz.Application.Contracts;
using Alborz.Application.Features.Products.Commands;
using Alborz.Infrastructure.Data;
using Alborz.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ProjectName.WinUI.ViewModels;
using System;

namespace Alborz.WinUI;

public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;
    public IServiceProvider Services { get; }

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();

        var dbContext = Services.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=Alborz.db"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IPurchaseReceiptRepository, PurchaseReceiptRepository>();
        services.AddTransient<PurchaseReceiptViewModel>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ProductCommandHandlers).Assembly));

        services.AddTransient<ProductsViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
