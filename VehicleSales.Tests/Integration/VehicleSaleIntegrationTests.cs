using FluentAssertions;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Enums;
using VehicleSales.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using VehicleSales.Tests.Base;
using VehicleSales.Application.Controllers;
using VehicleSales.Application.DTOs;

namespace VehicleSales.Tests.IntegrationTests;

/// <summary>
/// Testes de integração para funcionalidades que envolvem persistência de dados no MongoDB
/// Testam a integração entre a camada de domínio, aplicação e infraestrutura
/// </summary>
public class VehicleSaleIntegrationTests : TestBase
{
    #region Testes de Persistência

    /// <summary>
    /// Testa se uma venda é salva corretamente no MongoDB com todas as propriedades
    /// </summary>
    [Trait("Feature", "Persistence")]
    [Fact]
    public async Task Should_Save_VehicleSale_To_Database()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();
        
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            OriginalPrice = 85000.00m
        };
        
        var sale = new VehicleSale(Guid.NewGuid(), "12345678901", "Robert Anjos", "bob@email.com", 85000.00m, vehicleData);

        // Act
        var savedSale = await repository.CreateAsync(sale);

        // Assert
        var retrieved = await repository.GetByIdAsync(savedSale.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(sale.Id);
        retrieved.BuyerCpf.Should().Be("12345678901");
        retrieved.BuyerName.Should().Be("Robert Anjos");
        retrieved.BuyerEmail.Should().Be("bob@email.com");
        retrieved.SalePrice.Should().Be(85000.00m);
        retrieved.PaymentStatus.Should().Be(PaymentStatus.Pending);
        retrieved.PaymentCode.Should().NotBeNullOrEmpty();
        retrieved.VehicleData.Brand.Should().Be("Toyota");
        retrieved.VehicleData.Model.Should().Be("Corolla");

        _logger.LogInformation($"✅ Venda salva no MongoDB: {retrieved.VehicleData.Brand} {retrieved.VehicleData.Model} - Comprador: {retrieved.BuyerName}");
    }
    
    /// <summary>
    /// Testa se atualizações de vendas são persistidas corretamente
    /// </summary>
    [Trait("Feature", "Persistence")]
    [Fact]
    public async Task Should_Update_Sale_In_Database()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var vehicleData = new VehicleSnapshot
        {
            Brand = "Ford",
            Model = "Focus",
            Year = 2021,
            Color = "Azul",
            OriginalPrice = 70000.00m
        };
        
        var sale = new VehicleSale(Guid.NewGuid(), "11122233344", "Harumi A.", "harumi@email.com", 70000.00m, vehicleData);
        await repository.CreateAsync(sale);

        // Act
        sale.UpdatePaymentStatus(PaymentStatus.Paid);
        await repository.UpdateAsync(sale);

        // Assert
        var updated = await repository.GetByIdAsync(sale.Id);
        updated.Should().NotBeNull();
        updated!.PaymentStatus.Should().Be(PaymentStatus.Paid);
        updated.UpdatedAt.Should().NotBeNull();
        updated.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        _logger.LogInformation($"🔄 Venda atualizada no MongoDB: Status alterado para {updated.PaymentStatus}");
    }

    #endregion

    #region Testes de Consulta

    /// <summary>
    /// Testa se a consulta retorna apenas vendas com status específico
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Get_Sales_By_Payment_Status()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var vehicleData1 = new VehicleSnapshot { Brand = "Toyota", Model = "Corolla", Year = 2022, Color = "Prata", OriginalPrice = 85000.00m };
        var vehicleData2 = new VehicleSnapshot { Brand = "Honda", Model = "Civic", Year = 2022, Color = "Azul", OriginalPrice = 95000.00m };
        var vehicleData3 = new VehicleSnapshot { Brand = "Ford", Model = "Focus", Year = 2021, Color = "Preto", OriginalPrice = 70000.00m };

        var sale1 = new VehicleSale(Guid.NewGuid(), "11111111111", "Cliente 1", "cliente1@email.com", 85000.00m, vehicleData1);
        var sale2 = new VehicleSale(Guid.NewGuid(), "22222222222", "Cliente 2", "cliente2@email.com", 95000.00m, vehicleData2);
        var sale3 = new VehicleSale(Guid.NewGuid(), "33333333333", "Cliente 3", "cliente3@email.com", 70000.00m, vehicleData3);
        
        // Confirmar apenas as vendas 1 e 3
        sale1.UpdatePaymentStatus(PaymentStatus.Paid);
        sale3.UpdatePaymentStatus(PaymentStatus.Paid);

        await repository.CreateAsync(sale1);
        await repository.CreateAsync(sale2);
        await repository.CreateAsync(sale3);

        // Act
        var allSales = await repository.GetAllAsync();
        var paidSales = allSales.Where(s => s.PaymentStatus == PaymentStatus.Paid).ToList();
        var pendingSales = allSales.Where(s => s.PaymentStatus == PaymentStatus.Pending).ToList();

        // Assert
        paidSales.Should().HaveCount(2);
        paidSales.Should().Contain(s => s.VehicleData.Brand == "Toyota");
        paidSales.Should().Contain(s => s.VehicleData.Brand == "Ford");
        
        pendingSales.Should().HaveCount(1);
        pendingSales.Should().Contain(s => s.VehicleData.Brand == "Honda");

        _logger.LogInformation($"✅ Filtro por status: {paidSales.Count} pagas, {pendingSales.Count} pendentes");
    }

    /// <summary>
    /// Testa se a consulta por código de pagamento funciona corretamente
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Find_Sale_By_Payment_Code()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var vehicleData = new VehicleSnapshot
        {
            Brand = "Nissan",
            Model = "Sentra",
            Year = 2021,
            Color = "Cinza",
            OriginalPrice = 80000.00m
        };
        
        var sale = new VehicleSale(Guid.NewGuid(), "55566677788", "Ana Costa", "ana@email.com", 80000.00m, vehicleData);
        await repository.CreateAsync(sale);

        // Act
        var foundSale = await repository.GetByPaymentCodeAsync(sale.PaymentCode);

        // Assert
        foundSale.Should().NotBeNull();
        foundSale!.Id.Should().Be(sale.Id);
        foundSale.BuyerName.Should().Be("Ana Costa");
        foundSale.PaymentCode.Should().Be(sale.PaymentCode);

        _logger.LogInformation($"🔍 Venda encontrada por código: {foundSale.PaymentCode} - {foundSale.VehicleData.Brand} {foundSale.VehicleData.Model}");
    }

    /// <summary>
    /// Testa se a consulta por CPF do comprador funciona corretamente
    /// </summary>
    [Trait("Category", "Integration")]
    [Trait("Feature", "Querying")]
    [Fact]
    public async Task Should_Find_Sales_By_Buyer_Cpf()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVehicleSaleRepository>();

        var buyerCpf = "99988877766";
        var vehicleData1 = new VehicleSnapshot { Brand = "Toyota", Model = "Corolla", Year = 2022, Color = "Branco", OriginalPrice = 85000.00m };
        var vehicleData2 = new VehicleSnapshot { Brand = "Honda", Model = "Civic", Year = 2021, Color = "Prata", OriginalPrice = 95000.00m };

        // Mesmo CPF comprando dois veículos
        var sale1 = new VehicleSale(Guid.NewGuid(), buyerCpf, "Pedro Oliveira", "pedro@email.com", 85000.00m, vehicleData1);
        var sale2 = new VehicleSale(Guid.NewGuid(), buyerCpf, "Pedro Oliveira", "pedro@email.com", 95000.00m, vehicleData2);
        
        // Outro comprador
        var sale3 = new VehicleSale(Guid.NewGuid(), "12312312312", "Outro Cliente", "outro@email.com", 70000.00m, vehicleData1);

        await repository.CreateAsync(sale1);
        await repository.CreateAsync(sale2);
        await repository.CreateAsync(sale3);

        // Act
        var salesByBuyer = await repository.GetByBuyerCpfAsync(buyerCpf);

        // Assert
        salesByBuyer.Should().HaveCount(2);
        salesByBuyer.Should().AllSatisfy(s => s.BuyerCpf.Should().Be(buyerCpf));
        salesByBuyer.Should().AllSatisfy(s => s.BuyerName.Should().Be("Pedro Oliveira"));

        _logger.LogInformation($"👤 Vendas por CPF: {salesByBuyer.Count()} vendas para CPF {buyerCpf}");
    }

    #endregion

    #region Testes de Use Cases Completos

    /// <summary>
    /// Testa o fluxo completo de criação de venda através dos Use Cases
    /// </summary>
    [Trait("Feature", "FullFlow")]
    [Fact]
    public async Task Should_Create_Sale_Through_UseCase_Controller()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var useCaseController = scope.ServiceProvider.GetRequiredService<SaleUseCaseController>();
        
        var createSaleDto = new CreateSaleDto
        {
            VehicleId = Guid.NewGuid(),
            BuyerCpf = "12345678901",
            BuyerName = "Robert Anjos",
            BuyerEmail = "bob@email.com"
        };

        // Act
        var result = await useCaseController.CreateSale(createSaleDto);

        // Assert
        result.Should().NotBeNull();
        result.BuyerCpf.Should().Be("12345678901");
        result.BuyerName.Should().Be("Robert Anjos");
        result.BuyerEmail.Should().Be("bob@email.com");
        result.PaymentStatus.Should().Be("Pending");
        result.PaymentCode.Should().NotBeNullOrEmpty();
        result.VehicleData.Should().NotBeNull();

        _logger.LogInformation($"✅ Venda criada via Use Case: {result.PaymentCode} - {result.VehicleData.Brand} {result.VehicleData.Model}");
    }

    /// <summary>
    /// Testa o fluxo completo de processamento de webhook de pagamento
    /// </summary>
    [Trait("Feature", "PaymentWebhook")]
    [Fact]
    public async Task Should_Process_Payment_Webhook_Successfully()
    {
        // Arrange - Criar venda primeiro
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var useCaseController = scope.ServiceProvider.GetRequiredService<SaleUseCaseController>();
        
        var createSaleDto = new CreateSaleDto
        {
            VehicleId = Guid.NewGuid(),
            BuyerCpf = "98765432100",
            BuyerName = "Maria Santos",
            BuyerEmail = "maria@email.com"
        };

        var createdSale = await useCaseController.CreateSale(createSaleDto);

        // Act - Processar webhook
        var webhookDto = new PaymentWebhookDto
        {
            PaymentCode = createdSale.PaymentCode,
            Status = "Paid"
        };

        var webhookResult = await useCaseController.ProcessPayment(webhookDto);

        // Assert
        webhookResult.Should().BeTrue();

        // Verificar se foi atualizado no banco
        var updatedSale = await useCaseController.GetSaleById(createdSale.Id);
        updatedSale.Should().NotBeNull();
        updatedSale!.PaymentStatus.Should().Be("Paid");

        _logger.LogInformation($"🔗 Webhook processado: {createdSale.PaymentCode} → Paid");
    }

    /// <summary>
    /// Testa o cenário completo de cancelamento de pagamento via webhook
    /// </summary>
    [Trait("Feature", "PaymentWebhook")]
    [Fact]
    public async Task Should_Handle_Payment_Cancellation_Webhook()
    {
        // Arrange - Criar venda
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var useCaseController = scope.ServiceProvider.GetRequiredService<SaleUseCaseController>();
        
        var createSaleDto = new CreateSaleDto
        {
            VehicleId = Guid.NewGuid(),
            BuyerCpf = "11122233344",
            BuyerName = "Harumi A.",
            BuyerEmail = "harumi@email.com"
        };

        var createdSale = await useCaseController.CreateSale(createSaleDto);

        // Act - Cancelar via webhook
        var webhookDto = new PaymentWebhookDto
        {
            PaymentCode = createdSale.PaymentCode,
            Status = "Cancelled"
        };

        var webhookResult = await useCaseController.ProcessPayment(webhookDto);

        // Assert
        webhookResult.Should().BeTrue();

        var cancelledSale = await useCaseController.GetSaleById(createdSale.Id);
        cancelledSale.Should().NotBeNull();
        cancelledSale!.PaymentStatus.Should().Be("Cancelled");

        _logger.LogInformation($"🔄 Webhook cancelamento: {createdSale.PaymentCode} → Cancelled");
    }

    /// <summary>
    /// Testa o cenário de webhook com código de pagamento inexistente
    /// </summary>
    [Trait("Feature", "PaymentWebhook")]
    [Fact]
    public async Task Should_Return_False_For_NonExistent_Payment_Code()
    {
        // Arrange
        await CleanDatabaseAsync();
        using var scope = _serviceProvider.CreateScope();
        var useCaseController = scope.ServiceProvider.GetRequiredService<SaleUseCaseController>();

        var webhookDto = new PaymentWebhookDto
        {
            PaymentCode = "PAY-INEXISTENTE-123",
            Status = "Paid"
        };

        // Act
        var result = await useCaseController.ProcessPayment(webhookDto);

        // Assert
        result.Should().BeFalse();

        _logger.LogInformation($"❌ Webhook rejeitado: código inexistente {webhookDto.PaymentCode}");
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Cleanup após cada teste
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await TearDownAsync();
    }

    #endregion
}