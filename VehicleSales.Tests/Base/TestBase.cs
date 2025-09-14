using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Repositories;
using VehicleSales.Application.Gateways;
using VehicleSales.Infrastructure.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.Controllers;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Enums;
using VehicleSales.Tests.Mocks;

namespace VehicleSales.Tests.Base;

public abstract class TestBase
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IMongoDatabase _database;
    protected readonly IConfiguration _configuration;
    protected readonly ILogger _logger;

    protected TestBase()
    {
        // Configuração
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"MongoDbSettings:DatabaseName", $"vehicle_sales_test_{Guid.NewGuid():N}"}
            })
            .Build();

        var services = new ServiceCollection();
        
        // MongoDB InMemory (usando MongoDB.Driver.Core.TestHelpers ou Testcontainers)
        // Para simplicidade, usaremos uma instância local com nome único
        services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            var connectionString = "mongodb://localhost:27017"; // ou usar Testcontainers
            return new MongoClient(connectionString);
        });

        services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMongoClient>();
            var databaseName = _configuration.GetValue<string>("MongoDbSettings:DatabaseName");
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
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Configuration
        services.AddSingleton<IConfiguration>(_configuration);

        // Build do provider
        _serviceProvider = services.BuildServiceProvider();
        _database = _serviceProvider.GetRequiredService<IMongoDatabase>();
        _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
    }

    protected IServiceProvider GetServices()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider;
    }

    /// <summary>
    /// Limpar a collection antes de cada teste
    /// </summary>
    protected async Task CleanDatabaseAsync()
    {
        var collection = _database.GetCollection<VehicleSale>("vehicle_sales");
        await collection.DeleteManyAsync(FilterDefinition<VehicleSale>.Empty);
    }

    /// <summary>
    /// Criar dados básicos para testes
    /// </summary>
    protected async Task<List<VehicleSale>> SeedBasicDataAsync()
    {
        await CleanDatabaseAsync();

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        // Criar vendas usando o construtor correto
        var sales = new List<VehicleSale>
        {
            new VehicleSale(Guid.NewGuid(), "12345678901", "Robert Anjos", "bob@email.com", 85000.00m,
                new VehicleSnapshot { Brand = "Toyota", Model = "Corolla", Year = 2022, Color = "Prata", OriginalPrice = 85000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "98765432100", "Maria Santos", "maria@email.com", 70000.00m,
                new VehicleSnapshot { Brand = "Ford", Model = "Focus", Year = 2021, Color = "Preto", OriginalPrice = 70000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "11122233344", "Harumi A.", "harumi@email.com", 180000.00m,
                new VehicleSnapshot { Brand = "BMW", Model = "X1", Year = 2023, Color = "Branco", OriginalPrice = 180000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "55566677788", "Ana Costa", "ana@email.com", 95000.00m,
                new VehicleSnapshot { Brand = "Honda", Model = "Civic", Year = 2022, Color = "Azul", OriginalPrice = 95000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "99988877766", "Pedro Oliveira", "pedro@email.com", 80000.00m,
                new VehicleSnapshot { Brand = "Nissan", Model = "Sentra", Year = 2021, Color = "Cinza", OriginalPrice = 80000.00m })
        };

        foreach (var sale in sales)
        {
            await repository.CreateAsync(sale);
        }

        _logger.LogInformation($"Seed básico: {sales.Count} vendas criadas");
        return sales;
    }

    /// <summary>
    /// Criar cenário com vendas com status diferentes
    /// </summary>
    protected async Task<(List<VehicleSale> Pending, List<VehicleSale> Paid)> SeedMixedStatusAsync()
    {
        await CleanDatabaseAsync();

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        // Vendas pendentes
        var pendingSales = new List<VehicleSale>
        {
            new VehicleSale(Guid.NewGuid(), "12345678901", "Robert Anjos", "bob@email.com", 85000.00m,
                new VehicleSnapshot { Brand = "Toyota", Model = "Corolla", Year = 2022, Color = "Prata", OriginalPrice = 85000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "98765432100", "Maria Santos", "maria@email.com", 70000.00m,
                new VehicleSnapshot { Brand = "Ford", Model = "Focus", Year = 2021, Color = "Preto", OriginalPrice = 70000.00m })
        };

        // Vendas pagas
        var paidSales = new List<VehicleSale>
        {
            new VehicleSale(Guid.NewGuid(), "11122233344", "Harumi A.", "harumi@email.com", 180000.00m,
                new VehicleSnapshot { Brand = "BMW", Model = "X1", Year = 2023, Color = "Azul", OriginalPrice = 180000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "55566677788", "Ana Costa", "ana@email.com", 95000.00m,
                new VehicleSnapshot { Brand = "Honda", Model = "Civic", Year = 2022, Color = "Vermelho", OriginalPrice = 95000.00m })
        };

        // Atualizar status das vendas pagas
        foreach (var sale in paidSales)
        {
            sale.UpdatePaymentStatus(PaymentStatus.Paid);
        }

        // Salvar todas
        var allSales = pendingSales.Concat(paidSales).ToList();
        foreach (var sale in allSales)
        {
            await repository.CreateAsync(sale);
        }

        _logger.LogInformation($"Seed misto: {pendingSales.Count} pendentes, {paidSales.Count} pagas");
        return (pendingSales, paidSales);
    }

    /// <summary>
    /// Criar vendas para teste de ordenação por preço
    /// </summary>
    protected async Task<List<VehicleSale>> SeedPriceOrderTestAsync()
    {
        await CleanDatabaseAsync();

        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var sales = new List<VehicleSale>
        {
            new VehicleSale(Guid.NewGuid(), "11111111111", "Cliente 1", "cliente1@email.com", 250000.00m,
                new VehicleSnapshot { Brand = "BMW", Model = "X3", Year = 2023, Color = "Branco", OriginalPrice = 250000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "22222222222", "Cliente 2", "cliente2@email.com", 45000.00m,
                new VehicleSnapshot { Brand = "Ford", Model = "Ka", Year = 2021, Color = "Vermelho", OriginalPrice = 45000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "33333333333", "Cliente 3", "cliente3@email.com", 85000.00m,
                new VehicleSnapshot { Brand = "Toyota", Model = "Corolla", Year = 2022, Color = "Prata", OriginalPrice = 85000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "44444444444", "Cliente 4", "cliente4@email.com", 95000.00m,
                new VehicleSnapshot { Brand = "Honda", Model = "Civic", Year = 2022, Color = "Azul", OriginalPrice = 95000.00m }),
            
            new VehicleSale(Guid.NewGuid(), "55555555555", "Cliente 5", "cliente5@email.com", 35000.00m,
                new VehicleSnapshot { Brand = "Fiat", Model = "Uno", Year = 2020, Color = "Branco", OriginalPrice = 35000.00m })
        };

        foreach (var sale in sales)
        {
            await repository.CreateAsync(sale);
        }

        _logger.LogInformation($"Seed para ordenação: {sales.Count} vendas com preços variados");
        return sales;
    }

    /// <summary>
    /// Simular webhook de pagamento
    /// </summary>
    protected async Task<VehicleSale?> SimulatePaymentWebhookAsync(string paymentCode, PaymentStatus status)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var sale = await repository.GetByPaymentCodeAsync(paymentCode);
        if (sale != null)
        {
            sale.UpdatePaymentStatus(status);
            await repository.UpdateAsync(sale);
            _logger.LogInformation($"Webhook simulado: {paymentCode} → {status}");
        }
        return sale;
    }

    /// <summary>
    /// Limpar banco após os testes
    /// </summary>
    protected async Task TearDownAsync()
    {
        await CleanDatabaseAsync();
        
        // Para testes, pode dropar o database inteiro
        await _database.Client.DropDatabaseAsync(_database.DatabaseNamespace.DatabaseName);
    }
}