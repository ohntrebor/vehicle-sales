using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Data;
using VehicleSales.Infrastructure.Repositories;
using VehicleSales.Application.Gateways;
using VehicleSales.Infrastructure.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.Controllers;

namespace VehicleSales.Tests;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true);
        
        Configuration = builder.Build();
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        // DbContext com InMemory
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });
        
        // Repositories e Unit of Work 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        
        // Clean Architecture
        services.AddScoped<IVehicleGateway, VehicleGateway>();
        services.AddScoped<IVehiclePresenter, VehiclePresenter>();
        services.AddScoped<VehicleUseCaseController>();
        
        // Logging
        services.AddLogging();
        
        // Configuration
        services.AddSingleton<IConfiguration>(Configuration);
    }
}