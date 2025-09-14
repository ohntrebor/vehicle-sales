using FluentAssertions;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Enums;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace VehicleSales.Tests.UnitTests;

/// <summary>
/// Testes unitários para a entidade Vehicle
/// Focam apenas na lógica de domínio, sem dependências externas
/// </summary>
public class VehicleUnitTests
{
    private readonly ILogger<VehicleUnitTests> _logger;

    public VehicleUnitTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = loggerFactory.CreateLogger<VehicleUnitTests>();
    }

    #region Testes de Criação

    /// <summary>
    /// Testa se um veículo é criado corretamente através do construtor com todas as propriedades inicializadas
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleCreation")]
    [Fact]
    public void Should_Create_Vehicle_With_Valid_Properties()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var brand = "Toyota";
        var model = "Corolla";
        var year = 2022;
        var color = "Prata";
        var price = 85000.00m;

        // Act
        var vehicle = new Vehicle(brand, model, year, color, price);
        var afterCreation = DateTime.UtcNow;

        // Assert
        vehicle.Id.Should().NotBe(Guid.Empty);
        vehicle.Brand.Should().Be(brand);
        vehicle.Model.Should().Be(model);
        vehicle.Year.Should().Be(year);
        vehicle.Color.Should().Be(color);
        vehicle.Price.Should().Be(price);
        vehicle.IsSold.Should().BeFalse();
        vehicle.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        vehicle.CreatedAt.Should().BeOnOrBefore(afterCreation);
        vehicle.UpdatedAt.Should().BeNull();
        vehicle.BuyerCpf.Should().BeNull();
        vehicle.SaleDate.Should().BeNull();
        vehicle.PaymentCode.Should().BeNull();
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Pending);

        _logger.LogInformation($"✅ Veículo criado: {vehicle.Brand} {vehicle.Model} - R$ {vehicle.Price}");
    }

    /// <summary>
    /// Testa se cada veículo criado possui um ID único
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleCreation")]
    [Fact]
    public void Should_Generate_Unique_Id_For_Each_Vehicle()
    {
        // Act
        var vehicle1 = new Vehicle("Ford", "Ka", 2021, "Branco", 45000.00m);
        var vehicle2 = new Vehicle("Ford", "Ka", 2021, "Branco", 45000.00m);

        // Assert
        vehicle1.Id.Should().NotBe(vehicle2.Id);
        vehicle1.Id.Should().NotBe(Guid.Empty);
        vehicle2.Id.Should().NotBe(Guid.Empty);

        _logger.LogInformation($"✅ IDs únicos gerados: {vehicle1.Id} e {vehicle2.Id}");
    }

    #endregion

    #region Testes de Atualização

    /// <summary>
    /// Testa se os detalhes do veículo são atualizados corretamente e se o campo UpdatedAt é definido
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleUpdate")]
    [Fact]
    public void Should_Update_Vehicle_Details_Successfully()
    {
        // Arrange
        var vehicle = new Vehicle("Ford", "Focus", 2021, "Preto", 70000.00m);
        var originalCreatedAt = vehicle.CreatedAt;
        var originalId = vehicle.Id;

        // Act
        vehicle.UpdateDetails("Ford", "Focus", 2021, "Azul", 75000.00m);

        // Assert
        vehicle.Id.Should().Be(originalId); // ID não deve mudar
        vehicle.Brand.Should().Be("Ford");
        vehicle.Model.Should().Be("Focus");
        vehicle.Year.Should().Be(2021);
        vehicle.Color.Should().Be("Azul");
        vehicle.Price.Should().Be(75000.00m);
        vehicle.CreatedAt.Should().Be(originalCreatedAt); // CreatedAt não deve mudar
        vehicle.UpdatedAt.Should().NotBeNull();
        vehicle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _logger.LogInformation($"✅ Veículo atualizado: {vehicle.Brand} {vehicle.Model} - Nova cor: {vehicle.Color}");
    }

    /// <summary>
    /// Testa se é possível atualizar apenas o preço mantendo outras propriedades
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleUpdate")]
    [Fact]
    public void Should_Update_Only_Price_When_Other_Details_Same()
    {
        // Arrange
        var vehicle = new Vehicle("Honda", "Civic", 2022, "Prata", 90000.00m);
        var originalColor = vehicle.Color;

        // Act
        vehicle.UpdateDetails("Honda", "Civic", 2022, "Prata", 95000.00m);

        // Assert
        vehicle.Color.Should().Be(originalColor);
        vehicle.Price.Should().Be(95000.00m);
        vehicle.UpdatedAt.Should().NotBeNull();

        _logger.LogInformation($"✅ Apenas preço atualizado: R$ {vehicle.Price}");
    }

    #endregion

    #region Testes de Venda

    /// <summary>
    /// Testa se uma venda é registrada com sucesso, definindo todas as propriedades relacionadas à venda
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleSale")]
    [Fact]
    public void Should_Register_Sale_Successfully()
    {
        // Arrange
        var vehicle = new Vehicle("BMW", "X3", 2023, "Branco", 250000.00m);
        var buyerCpf = "12345678901";
        var paymentCode = "PAY-123456";
        var befoSales = DateTime.UtcNow;

        // Act
        vehicle.RegisterSale(buyerCpf, paymentCode);
        var afterSale = DateTime.UtcNow;

        // Assert
        vehicle.IsSold.Should().BeTrue();
        vehicle.BuyerCpf.Should().Be(buyerCpf);
        vehicle.PaymentCode.Should().Be(paymentCode);
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Pending);
        vehicle.SaleDate.Should().BeOnOrAfter(befoSales);
        vehicle.SaleDate.Should().BeOnOrBefore(afterSale);
        vehicle.UpdatedAt.Should().NotBeNull();

        _logger.LogInformation($"💰 Venda registrada: {vehicle.Brand} {vehicle.Model} - Comprador: {buyerCpf}");
    }

    /// <summary>
    /// Testa se o sistema impede a venda de um veículo que já foi vendido
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleSale")]
    [Fact]
    public void Should_Not_Allow_Sale_If_Already_Sold()
    {
        // Arrange
        var vehicle = new Vehicle("Honda", "Civic", 2022, "Azul", 95000.00m);
        vehicle.RegisterSale("11111111111", "PAY-111");

        // Act & Assert
        var action = () => vehicle.RegisterSale("22222222222", "PAY-222");
        action.Should().Throw<InvalidOperationException>()
              .WithMessage("Veículo já está vendido");

        // O estado original deve ser mantido
        vehicle.BuyerCpf.Should().Be("11111111111");
        vehicle.PaymentCode.Should().Be("PAY-111");

        _logger.LogInformation($"✅ Validação funcionando: venda duplicada bloqueada");
    }

    /// <summary>
    /// Testa se argumentos nulos ou vazios são rejeitados na venda
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "VehicleSale")]
    [Theory]
    [InlineData(null, "PAY-123")]
    [InlineData("", "PAY-123")]
    [InlineData("12345678901", null)]
    [InlineData("12345678901", "")]
    public void Should_Not_Allow_Sale_With_Invalid_Parameters(string buyerCpf, string paymentCode)
    {
        // Arrange
        var vehicle = new Vehicle("Toyota", "Camry", 2022, "Preto", 120000.00m);

        // Act & Assert
        var action = () => vehicle.RegisterSale(buyerCpf, paymentCode);
        action.Should().Throw<ArgumentException>();

        vehicle.IsSold.Should().BeFalse();
        vehicle.BuyerCpf.Should().BeNull();
        vehicle.PaymentCode.Should().BeNull();

        _logger.LogInformation($"✅ Validação funcionando: parâmetros inválidos rejeitados");
    }

    #endregion

    #region Testes de Status de Pagamento

    /// <summary>
    /// Testa se o status de pagamento é atualizado corretamente para confirmado
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Update_Payment_Status_To_Confirmed()
    {
        // Arrange
        var vehicle = new Vehicle("Nissan", "Sentra", 2021, "Cinza", 85000.00m);
        vehicle.RegisterSale("33333333333", "PAY-333");

        // Act
        vehicle.UpdatePaymentStatus("g51dg5fdfg1", PaymentStatus.Confirmed);

        // Assert
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Confirmed);
        vehicle.IsSold.Should().BeTrue(); // Deve continuar vendido
        vehicle.BuyerCpf.Should().Be("33333333333"); // Dados de venda mantidos
        vehicle.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _logger.LogInformation($"✅ Pagamento confirmado: {vehicle.Brand} {vehicle.Model}");
    }

    /// <summary>
    /// Testa se a venda é cancelada corretamente quando o pagamento é cancelado, revertendo o status de venda
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Cancel_Sale_When_Payment_Cancelled()
    {
        // Arrange
        var vehicle = new Vehicle("Volkswagen", "Golf", 2020, "Vermelho", 75000.00m);
        vehicle.RegisterSale("44444444444", "PAY-444");

        // Act
        vehicle.UpdatePaymentStatus("51fg51dg5fdfg1", PaymentStatus.Cancelled);

        // Assert
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        vehicle.IsSold.Should().BeFalse(); // Deve reverter venda
        vehicle.BuyerCpf.Should().BeNull();
        vehicle.SaleDate.Should().BeNull();
        vehicle.PaymentCode.Should().BeNull();

        _logger.LogInformation($"🔄 Venda cancelada: {vehicle.Brand} {vehicle.Model} - Veículo disponível novamente");
    }

    /// <summary>
    /// Testa transições de status de pagamento de pending para confirmed e depois para cancelled
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Feature", "PaymentStatus")]
    [Fact]
    public void Should_Handle_Payment_Status_Transitions()
    {
        // Arrange
        var vehicle = new Vehicle("Audi", "A3", 2022, "Prata", 150000.00m);
        
        // Act & Assert - Registrar venda
        vehicle.RegisterSale("99999999999", "PAY-999");
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Pending);
        vehicle.IsSold.Should().BeTrue();

        // Act & Assert - Confirmar pagamento
        vehicle.UpdatePaymentStatus("51fg51dg5fdfg1", PaymentStatus.Confirmed);
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Confirmed);
        vehicle.IsSold.Should().BeTrue();

        // Act & Assert - Cancelar pagamento
        vehicle.UpdatePaymentStatus("51fg51dg5fdfg1", PaymentStatus.Cancelled);
        vehicle.PaymentStatus.Should().Be(PaymentStatus.Cancelled);
        vehicle.IsSold.Should().BeFalse();

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
        var vehicle = new Vehicle("Chevrolet", "Onix", 2021, "Vermelho", 55999.99m);

        // Assert
        vehicle.Price.Should().Be(55999.99m);

        // Act - Atualizar com novo preço decimal
        vehicle.UpdateDetails("Chevrolet", "Onix", 2021, "Vermelho", 57500.50m);

        // Assert
        vehicle.Price.Should().Be(57500.50m);

        _logger.LogInformation($"✅ Preços decimais tratados corretamente: R$ {vehicle.Price}");
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
        var vehicle = new Vehicle("Mercedes-Benz", "Classe A", 2023, "Azul Metálico", 200000.00m);

        // Assert
        vehicle.Brand.Should().Be("Mercedes-Benz");
        vehicle.Model.Should().Be("Classe A");
        vehicle.Color.Should().Be("Azul Metálico");

        _logger.LogInformation($"✅ Caracteres especiais tratados: {vehicle.Brand} {vehicle.Model}");
    }

    #endregion
}
