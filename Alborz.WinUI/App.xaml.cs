using Alborz.Application.Contracts;
using Alborz.Application.Features.Products.Commands;
using Alborz.Infrastructure.Data;
using Alborz.Infrastructure.Repositories;
using Alborz.WinUI.ViewModels.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
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
            options.UseSqlite("Data Source=Alborz.db"),
            ServiceLifetime.Transient);

        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<IInvoiceRepository, InvoiceRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

        services.AddTransient<ProductsViewModel>();

        return services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
