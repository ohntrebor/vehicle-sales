using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Repositories;
using VehicleSales.Application.Gateways;
using VehicleSales.Infrastructure.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.Controllers;
using VehicleSales.Tests.Mocks;

namespace VehicleSales.Tests;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true)
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"ConnectionStrings:MongoDb", "mongodb+srv://vehicle-sales:<db_password>@vehicle-sales.rx54kko.mongodb.net/"},
                {"MongoDbSettings:DatabaseName", "vehicle_sales_test"}
            });
        
        Configuration = builder.Build();
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        // MongoDB Configuration
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = Configuration.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        });

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var databaseName = $"vehicle_sales_test_{Guid.NewGuid():N}"; // Database único por teste
            return client.GetDatabase(databaseName);
        });
        
        // Repositories
        services.AddScoped<IVehicleSaleRepository, VehicleSaleRepository>();
        
        // Clean Architecture Services
        services.AddScoped<ISaleGateway, SaleGateway>();
        services.AddScoped<ISalePresenter, SalePresenter>();
        services.AddScoped<SaleUseCaseController>();
        
        // Mock do VehicleCatalogService para testes
        services.AddScoped<IVehicleCatalogService, MockVehicleCatalogService>();
        
        // Logging
        services.AddLogging();
        
        // Configuration
        services.AddSingleton<IConfiguration>(Configuration);
    }
}