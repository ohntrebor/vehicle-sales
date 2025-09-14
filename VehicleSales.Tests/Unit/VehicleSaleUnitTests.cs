using FluentAssertions;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Enums;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace VehicleSales.Tests.UnitTests;

/// <summary>
/// Testes unitários para a entidade VehicleSale
/// Focam apenas na lógica de domínio, sem dependências externas
/// </summary>
public class VehicleSaleUnitTests
{
    private readonly ILogger<VehicleSaleUnitTests> _logger;

    public VehicleSaleUnitTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<VehicleSaleUnitTests>();
    }

    #region Testes de Criação

    /// <summary>
    /// Testa se uma venda é criada corretamente através do construtor com todas as propriedades inicializadas
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "SaleCreation")]
    [Fact]
    public void Should_Create_VehicleSale_With_Valid_Properties()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var vehicleId = Guid.NewGuid();
        var buyerCpf = "12345678901";
        var buyerName = "Robert Anjos";
        var buyerEmail = "bob@email.com";
        var salePrice = 85000.00m;
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Toyota",
            Model = "Corolla",
            Year = 2022,
            Color = "Prata",
            OriginalPrice = 85000.00m
        };

        // Act
        var sale = new VehicleSale(vehicleId, buyerCpf, buyerName, buyerEmail, salePrice, vehicleData);
        var afterCreation = DateTime.UtcNow;

        // Assert
        sale.Id.Should().NotBe(Guid.Empty);
        sale.VehicleId.Should().Be(vehicleId);
        sale.BuyerCpf.Should().Be(buyerCpf);
        sale.BuyerName.Should().Be(buyerName);
        sale.BuyerEmail.Should().Be(buyerEmail);
        sale.SalePrice.Should().Be(salePrice);
        sale.PaymentStatus.Should().Be(PaymentStatus.Pending);
        sale.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        sale.CreatedAt.Should().BeOnOrBefore(afterCreation);
        sale.UpdatedAt.Should().BeNull();
        sale.PaymentCode.Should().NotBeNullOrEmpty();
        sale.PaymentCode.Should().StartWith("PAY-");
        sale.VehicleData.Should().NotBeNull();
        sale.VehicleData.Brand.Should().Be("Toyota");

        _logger.LogInformation($"✅ Venda criada: {sale.VehicleData.Brand} {sale.VehicleData.Model} - Comprador: {sale.BuyerName}");
    }

    /// <summary>
    /// Testa se cada venda criada possui um ID único e um código de pagamento único
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "SaleCreation")]
    [Fact]
    public void Should_Generate_Unique_Id_And_PaymentCode_For_Each_Sale()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Ford",
            Model = "Ka",
            Year = 2021,
            Color = "Branco",
            OriginalPrice = 45000.00m
        };

        // Act
        var sale1 = new VehicleSale(Guid.NewGuid(), "11111111111", "Cliente 1", "cliente1@email.com", 45000.00m, vehicleData);
        var sale2 = new VehicleSale(Guid.NewGuid(), "22222222222", "Cliente 2", "cliente2@email.com", 45000.00m, vehicleData);

        // Assert
        sale1.Id.Should().NotBe(sale2.Id);
        sale1.PaymentCode.Should().NotBe(sale2.PaymentCode);
        sale1.Id.Should().NotBe(Guid.Empty);
        sale2.Id.Should().NotBe(Guid.Empty);

        _logger.LogInformation($"✅ IDs únicos gerados: {sale1.Id} e {sale2.Id}");
        _logger.LogInformation($"✅ Códigos únicos gerados: {sale1.PaymentCode} e {sale2.PaymentCode}");
    }

    /// <summary>
    /// Testa se o código de pagamento é gerado no formato correto
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "SaleCreation")]
    [Fact]
    public void Should_Generate_PaymentCode_In_Correct_Format()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Honda",
            Model = "Civic",
            Year = 2022,
            Color = "Azul",
            OriginalPrice = 95000.00m
        };

        // Act
        var sale = new VehicleSale(Guid.NewGuid(), "33333333333", "Maria Santos", "maria@email.com", 95000.00m, vehicleData);

        // Assert
        sale.PaymentCode.Should().StartWith("PAY-");
        sale.PaymentCode.Should().HaveLength(21); // PAY-YYYYMMDD-8chars
        sale.PaymentCode.Should().MatchRegex(@"PAY-\d{8}-[A-Z0-9]{8}");

        _logger.LogInformation($"✅ Código de pagamento gerado: {sale.PaymentCode}");
    }

    #endregion

    #region Testes de Atualização de Status

    /// <summary>
    /// Testa se o status de pagamento é atualizado corretamente para Paid
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Update_Payment_Status_To_Paid()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "BMW",
            Model = "X1",
            Year = 2023,
            Color = "Branco",
            OriginalPrice = 180000.00m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "44444444444", "Harumi A.", "harumi@email.com", 180000.00m, vehicleData);
        var beforeUpdate = DateTime.UtcNow;

        // Act
        sale.UpdatePaymentStatus(PaymentStatus.Paid);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        sale.PaymentStatus.Should().Be(PaymentStatus.Paid);
        sale.UpdatedAt.Should().NotBeNull();
        sale.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        sale.UpdatedAt.Should().BeOnOrBefore(afterUpdate);

        _logger.LogInformation($"✅ Pagamento confirmado: {sale.VehicleData.Brand} {sale.VehicleData.Model}");
    }

    /// <summary>
    /// Testa se o status de pagamento pode ser atualizado para Cancelled
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Update_Payment_Status_To_Cancelled()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Volkswagen",
            Model = "Polo",
            Year = 2022,
            Color = "Vermelho",
            OriginalPrice = 65000.00m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "55555555555", "Ana Costa", "ana@email.com", 65000.00m, vehicleData);

        // Act
        sale.UpdatePaymentStatus(PaymentStatus.Cancelled);

        // Assert
        sale.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        sale.UpdatedAt.Should().NotBeNull();

        _logger.LogInformation($"🔄 Pagamento cancelado: {sale.VehicleData.Brand} {sale.VehicleData.Model}");
    }

    /// <summary>
    /// Testa se o status de pagamento pode ser atualizado para Failed
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Update_Payment_Status_To_Failed()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Nissan",
            Model = "Sentra",
            Year = 2021,
            Color = "Cinza",
            OriginalPrice = 80000.00m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "66666666666", "Pedro Oliveira", "pedro@email.com", 80000.00m, vehicleData);

        // Act
        sale.UpdatePaymentStatus(PaymentStatus.Failed);

        // Assert
        sale.PaymentStatus.Should().Be(PaymentStatus.Failed);
        sale.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _logger.LogInformation($"❌ Pagamento falhado: {sale.VehicleData.Brand} {sale.VehicleData.Model}");
    }

    /// <summary>
    /// Testa transições de status de pagamento de Pending para Paid e depois para Cancelled
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Handle_Payment_Status_Transitions()
    {
        // Arrange
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Audi",
            Model = "A3",
            Year = 2023,
            Color = "Azul",
            OriginalPrice = 165000.00m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "77777777777", "Fernanda Silva", "fernanda@email.com", 165000.00m, vehicleData);

        // Act & Assert - Status inicial
        sale.PaymentStatus.Should().Be(PaymentStatus.Pending);

        // Act & Assert - Confirmar pagamento
        sale.UpdatePaymentStatus(PaymentStatus.Paid);
        sale.PaymentStatus.Should().Be(PaymentStatus.Paid);

        // Act & Assert - Cancelar pagamento
        sale.UpdatePaymentStatus(PaymentStatus.Cancelled);
        sale.PaymentStatus.Should().Be(PaymentStatus.Cancelled);

        _logger.LogInformation($"✅ Transições de status testadas com sucesso");
    }

    #endregion

    #region Testes de Edge Cases

    /// <summary>
    /// Testa se valores decimais são tratados corretamente no preço
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "EdgeCases")]
    [Fact]
    public void Should_Handle_Decimal_Prices_Correctly()
    {
        // Arrange & Act
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Chevrolet",
            Model = "Onix",
            Year = 2021,
            Color = "Vermelho",
            OriginalPrice = 55999.99m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "99999999999", "Cliente Final", "final@email.com", 55999.99m, vehicleData);

        // Assert
        sale.SalePrice.Should().Be(55999.99m);
        sale.VehicleData.OriginalPrice.Should().Be(55999.99m);

        _logger.LogInformation($"✅ Preços decimais tratados corretamente: R$ {sale.SalePrice}");
    }

    /// <summary>
    /// Testa comportamento com strings com espaços e caracteres especiais
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "EdgeCases")]
    [Fact]
    public void Should_Handle_Special_Characters_In_Strings()
    {
        // Arrange & Act
        var vehicleData = new VehicleSnapshot
        {
            Brand = "Mercedes-Benz",
            Model = "Classe A",
            Year = 2023,
            Color = "Azul Metálico",
            OriginalPrice = 200000.00m
        };
        var sale = new VehicleSale(Guid.NewGuid(), "10101010101", "José da Silva-Santos", "jose.silva@email.com.br", 200000.00m, vehicleData);

        // Assert
        sale.BuyerName.Should().Be("José da Silva-Santos");
        sale.BuyerEmail.Should().Be("jose.silva@email.com.br");
        sale.VehicleData.Brand.Should().Be("Mercedes-Benz");
        sale.VehicleData.Model.Should().Be("Classe A");
        sale.VehicleData.Color.Should().Be("Azul Metálico");

        _logger.LogInformation($"✅ Caracteres especiais tratados: {sale.VehicleData.Brand} {sale.VehicleData.Model} - Comprador: {sale.BuyerName}");
    }
    
    #endregion
}