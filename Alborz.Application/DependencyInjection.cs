using Microsoft.Extensions.DependencyInjection;

namespace Alborz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<Features.Invoices.Services.InvoiceWriter>();
        services.AddScoped<Features.PurchaseReceipts.Services.PurchaseReceiptWriter>();
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        return services;
    }
}

