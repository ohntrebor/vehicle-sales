using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using VehicleSales.Infrastructure.Data;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Repositories;
using System.Reflection;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Tests.Base;

public abstract class TestBase
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ApplicationDbContext _db;
    protected readonly IConfiguration _configuration;
    protected readonly ILogger _logger;

    protected TestBase()
    {
        // Configuração
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: true)
            .Build();

        var services = new ServiceCollection();
        
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
        

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.Load("VehicleSales.Application"));
        });

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Configuration
        services.AddSingleton<IConfiguration>(_configuration);

        // Build do provider
        _serviceProvider = services.BuildServiceProvider();
        _db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
    }

    protected IServiceProvider GetServices()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider;
    }

    /// <summary>
    /// Criar dados básicos para testes
    /// </summary>
    protected async Task<List<Vehicle>> SeedBasicDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Limpar dados existentes
        context.RemoveRange(context.Vehicles);
        await context.SaveChangesAsync();

        // Criar veículos usando SEU construtor
        var vehicles = new List<Vehicle>
        {
            new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m),
            new Vehicle("Ford", "Focus", 2021, "Preto", 70000.00m),
            new Vehicle("BMW", "X1", 2023, "Branco", 180000.00m),
            new Vehicle("Honda", "Civic", 2022, "Azul", 95000.00m),
            new Vehicle("Nissan", "Sentra", 2021, "Cinza", 80000.00m)
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        _logger.LogInformation($"✅ Seed básico: {vehicles.Count} veículos criados");
        return vehicles;
    }

    /// <summary>
    /// Criar cenário com veículos vendidos e disponíveis
    /// </summary>
    protected async Task<(List<Vehicle> Available, List<Vehicle> Sold)> SeedMixedDataAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Limpar dados
        context.RemoveRange(context.Vehicles);
        await context.SaveChangesAsync();

        // Veículos disponíveis
        var availableVehicles = new List<Vehicle>
        {
            new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m),
            new Vehicle("Ford", "Focus", 2021, "Preto", 70000.00m),
            new Vehicle("Volkswagen", "Polo", 2023, "Branco", 65000.00m)
        };

        // Veículos vendidos
        var soldVehicles = new List<Vehicle>
        {
            new Vehicle("BMW", "X1", 2023, "Azul", 180000.00m),
            new Vehicle("Honda", "Civic", 2022, "Vermelho", 95000.00m)
        };

        // Registrar vendas
        soldVehicles[0].RegisterSale("11111111111", "PAY-BMW001");
        soldVehicles[1].RegisterSale("22222222222", "PAY-HONDA001");

        // Salvar todos
        var allVehicles = availableVehicles.Concat(soldVehicles).ToList();
        context.Vehicles.AddRange(allVehicles);
        await context.SaveChangesAsync();

        _logger.LogInformation($"✅ Seed misto: {availableVehicles.Count} disponíveis, {soldVehicles.Count} vendidos");
        return (availableVehicles, soldVehicles);
    }

    /// <summary>
    /// Criar veículos para teste de ordenação por preço
    /// </summary>
    protected async Task<List<Vehicle>> SeedPriceOrderTestAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.RemoveRange(context.Vehicles);
        await context.SaveChangesAsync();

        var vehicles = new List<Vehicle>
        {
            new Vehicle("BMW", "X3", 2023, "Branco", 250000.00m),      // Mais caro
            new Vehicle("Ford", "Ka", 2021, "Vermelho", 45000.00m),    // Mais barato  
            new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m), // Meio
            new Vehicle("Honda", "Civic", 2022, "Azul", 95000.00m),    // Meio-alto
            new Vehicle("Fiat", "Uno", 2020, "Branco", 35000.00m)      // Mais barato ainda
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        _logger.LogInformation($"✅ Seed para ordenação: {vehicles.Count} veículos com preços variados");
        return vehicles;
    }

    /// <summary>
    /// Simular webhook de pagamento
    /// </summary>
    protected async Task<Vehicle> SimulatePaymentWebhookAsync(string paymentCode, VehicleSales.Domain.Enums.PaymentStatus status)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.PaymentCode == paymentCode);
        if (vehicle != null)
        {
            vehicle.UpdatePaymentStatus(paymentCode, status);
            await context.SaveChangesAsync();
            _logger.LogInformation($"🔗 Webhook simulado: {paymentCode} → {status}");
        }
        return vehicle;
    }
}
