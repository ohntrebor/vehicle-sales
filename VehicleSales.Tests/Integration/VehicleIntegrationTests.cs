using FluentAssertions;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VehicleSales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using VehicleSales.Tests.Base;

namespace VehicleResale.Tests.IntegrationTests;

/// <summary>
/// Testes de integração para funcionalidades que envolvem persistência de dados
/// Testam a integração entre a camada de domínio e infraestrutura
/// </summary>
public class VehicleTests : TestBase
{
    #region Testes de Persistência

    /// <summary>
    /// Testa se um veículo é salvo corretamente no banco de dados com todas as propriedades
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Persistence")]
    [Fact]
    public async Task Should_Save_Vehicle_To_Database()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m);

        // Act
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Vehicles.FindAsync(vehicle.Id);
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(vehicle.Id);
        saved.Brand.Should().Be("Toyota");
        saved.Model.Should().Be("Corolla");
        saved.Year.Should().Be(2022);
        saved.Color.Should().Be("Prata");
        saved.Price.Should().Be(85000.00m);
        saved.IsSold.Should().BeFalse();
        saved.CreatedAt.Should().BeCloseTo(vehicle.CreatedAt, TimeSpan.FromSeconds(1));

        _logger.LogInformation($"✅ Veículo salvo no banco: {saved.Brand} {saved.Model}");
    }

    /// <summary>
    /// Testa se um veículo vendido é persistido corretamente com todos os dados de venda
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Persistence")]
    [Fact]
    public async Task Should_Save_Sold_Vehicle_With_Sale_Data()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle = new Vehicle("BMW", "X1", 2023, "Preto", 180000.00m);
        vehicle.RegisterSale("12345678901", "PAY-12345");

        // Act
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Vehicles.FindAsync(vehicle.Id);
        saved.Should().NotBeNull();
        saved!.IsSold.Should().BeTrue();
        saved.BuyerCpf.Should().Be("12345678901");
        saved.PaymentCode.Should().Be("PAY-12345");
        saved.PaymentStatus.Should().Be(PaymentStatus.Pending);
        saved.SaleDate.Should().NotBeNull();

        _logger.LogInformation($"💰 Veículo vendido salvo: {saved.Brand} {saved.Model} - Comprador: {saved.BuyerCpf}");
    }

    /// <summary>
    /// Testa se atualizações de veículos são persistidas corretamente
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Persistence")]
    [Fact]
    public async Task Should_Update_Vehicle_In_Database()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle = new Vehicle("Ford", "Focus", 2021, "Azul", 70000.00m);
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Act
        vehicle.UpdateDetails("Ford", "Focus", 2021, "Vermelho", 75000.00m);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Vehicles.FindAsync(vehicle.Id);
        updated.Should().NotBeNull();
        updated!.Color.Should().Be("Vermelho");
        updated.Price.Should().Be(75000.00m);
        updated.UpdatedAt.Should().NotBeNull();

        _logger.LogInformation($"🔄 Veículo atualizado no banco: {updated.Brand} {updated.Model}");
    }

    #endregion

    #region Testes de Consulta

    /// <summary>
    /// Testa se a consulta retorna apenas veículos disponíveis (não vendidos)
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Get_Available_Vehicles_Only()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle1 = new Vehicle("Ford", "Focus", 2021, "Preto", 70000.00m);
        var vehicle2 = new Vehicle("BMW", "X1", 2022, "Branco", 180000.00m);
        var vehicle3 = new Vehicle("Honda", "Civic", 2022, "Prata", 95000.00m);
        
        // Vender apenas o BMW
        vehicle2.RegisterSale("12345678901", "PAY-123");

        context.Vehicles.AddRange(vehicle1, vehicle2, vehicle3);
        await context.SaveChangesAsync();

        // Act
        var availableVehicles = await context.Vehicles
            .Where(v => !v.IsSold)
            .ToListAsync();

        // Assert
        availableVehicles.Should().HaveCount(2);
        availableVehicles.Should().Contain(v => v.Brand == "Ford");
        availableVehicles.Should().Contain(v => v.Brand == "Honda");
        availableVehicles.Should().NotContain(v => v.Brand == "BMW");

        _logger.LogInformation($"✅ Filtro disponíveis: {availableVehicles.Count} veículos encontrados");
    }

    /// <summary>
    /// Testa se a consulta retorna apenas veículos vendidos com suas informações de venda
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Get_Sold_Vehicles_Only()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle1 = new Vehicle("Honda", "Civic", 2022, "Azul", 95000.00m);
        var vehicle2 = new Vehicle("Nissan", "Sentra", 2021, "Cinza", 85000.00m);
        var vehicle3 = new Vehicle("Toyota", "Corolla", 2022, "Branco", 90000.00m);
        
        // Vender apenas Honda e Toyota
        vehicle1.RegisterSale("11111111111", "PAY-111");
        vehicle3.RegisterSale("33333333333", "PAY-333");

        context.Vehicles.AddRange(vehicle1, vehicle2, vehicle3);
        await context.SaveChangesAsync();

        // Act
        var soldVehicles = await context.Vehicles
            .Where(v => v.IsSold)
            .ToListAsync();

        // Assert
        soldVehicles.Should().HaveCount(2);
        soldVehicles.Should().Contain(v => v.Brand == "Honda" && v.BuyerCpf == "11111111111");
        soldVehicles.Should().Contain(v => v.Brand == "Toyota" && v.BuyerCpf == "33333333333");
        soldVehicles.Should().NotContain(v => v.Brand == "Nissan");
        soldVehicles.All(v => v.PaymentStatus == PaymentStatus.Pending).Should().BeTrue();

        _logger.LogInformation($"✅ Filtro vendidos: {soldVehicles.Count} veículos encontrados");
    }

    /// <summary>
    /// Testa se a consulta ordena os veículos corretamente por preço (menor para maior)
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Order_Vehicles_By_Price()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicles = new[]
        {
            new Vehicle("BMW", "X3", 2023, "Branco", 250000.00m),
            new Vehicle("Ford", "Focus", 2021, "Preto", 70000.00m),
            new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m),
            new Vehicle("Volkswagen", "Polo", 2020, "Azul", 55000.00m)
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        // Act
        var orderedByPrice = await context.Vehicles
            .Where(v => !v.IsSold)
            .OrderBy(v => v.Price)
            .ToListAsync();

        // Assert
        orderedByPrice.Should().HaveCount(4);
        orderedByPrice[0].Price.Should().Be(55000.00m); // VW Polo
        orderedByPrice[1].Price.Should().Be(70000.00m); // Ford Focus
        orderedByPrice[2].Price.Should().Be(85000.00m); // Toyota Corolla
        orderedByPrice[3].Price.Should().Be(250000.00m); // BMW X3

        _logger.LogInformation($"✅ Ordenação por preço: {string.Join(" → ", orderedByPrice.Select(v => $"R$ {v.Price}"))}");
    }

    /// <summary>
    /// Testa consultas complexas com múltiplos filtros
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Filter_By_Multiple_Criteria()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicles = new[]
        {
            new Vehicle("Toyota", "Corolla", 2022, "Prata", 85000.00m),
            new Vehicle("Toyota", "Camry", 2023, "Preto", 120000.00m),
            new Vehicle("Honda", "Civic", 2022, "Branco", 95000.00m),
            new Vehicle("Ford", "Focus", 2021, "Azul", 70000.00m)
        };

        context.Vehicles.AddRange(vehicles);
        await context.SaveChangesAsync();

        // Act - Buscar Toyotas de 2022+ com preço até 100k
        var filteredVehicles = await context.Vehicles
            .Where(v => v.Brand == "Toyota" && 
                       v.Year >= 2022 && 
                       v.Price <= 100000.00m &&
                       !v.IsSold)
            .ToListAsync();

        // Assert
        filteredVehicles.Should().HaveCount(1);
        filteredVehicles.Single().Model.Should().Be("Corolla");

        _logger.LogInformation($"✅ Filtro complexo: {filteredVehicles.Count} veículos encontrados");
    }

    #endregion

    #region Testes de Webhook/Cenários Reais

    /// <summary>
    /// Testa o cenário completo de webhook de pagamento, desde a venda até a confirmação
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "PaymentWebhook")]
    [Fact]
    public async Task Should_Handle_Payment_Webhook_Confirmation_Scenario()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle = new Vehicle("Mercedes", "A200", 2023, "Preto", 180000.00m);
        var paymentCode = "PAY-WEBHOOK-123";
        
        vehicle.RegisterSale("55555555555", paymentCode);
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Act - Simular webhook de confirmação
        var vehicleToUpdate = await context.Vehicles
            .FirstAsync(v => v.PaymentCode == paymentCode);
        vehicleToUpdate.UpdatePaymentStatus("51fg51dg5fdfg1", PaymentStatus.Confirmed);
        await context.SaveChangesAsync();

        // Assert
        var confirmedVehicle = await context.Vehicles.FindAsync(vehicle.Id);
        confirmedVehicle.Should().NotBeNull();
        confirmedVehicle!.PaymentStatus.Should().Be(PaymentStatus.Confirmed);
        confirmedVehicle.IsSold.Should().BeTrue();
        confirmedVehicle.BuyerCpf.Should().Be("55555555555");

        _logger.LogInformation($"🔗 Webhook confirmação simulado: Pagamento {paymentCode} confirmado para {confirmedVehicle.Brand} {confirmedVehicle.Model}");
    }

    /// <summary>
    /// Testa o cenário completo de webhook de cancelamento de pagamento
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "PaymentWebhook")]
    [Fact]
    public async Task Should_Handle_Payment_Webhook_Cancellation_Scenario()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var vehicle = new Vehicle("Audi", "A3", 2022, "Prata", 160000.00m);
        var paymentCode = "PAY-CANCEL-456";
        
        vehicle.RegisterSale("66666666666", paymentCode);
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();

        // Act - Simular webhook de cancelamento
        var vehicleToUpdate = await context.Vehicles
            .FirstAsync(v => v.PaymentCode == paymentCode);
        vehicleToUpdate.UpdatePaymentStatus("51fg51dg5fdfg1", PaymentStatus.Cancelled);
        await context.SaveChangesAsync();

        // Assert
        var cancelledVehicle = await context.Vehicles.FindAsync(vehicle.Id);
        cancelledVehicle.Should().NotBeNull();
        cancelledVehicle!.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        cancelledVehicle.IsSold.Should().BeFalse();
        cancelledVehicle.BuyerCpf.Should().BeNull();
        cancelledVehicle.SaleDate.Should().BeNull();
        cancelledVehicle.PaymentCode.Should().BeNull();

        _logger.LogInformation($"🔄 Webhook cancelamento simulado: Veículo {cancelledVehicle.Brand} {cancelledVehicle.Model} disponível novamente");
    }
    #endregion
}