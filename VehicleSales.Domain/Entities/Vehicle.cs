using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleSales.Domain.Enums;

namespace VehicleSales.Domain.Entities;

[Table("vehicles")]
/// <summary>
/// Entidade que representa um veículo no sistema
/// </summary>
public class Vehicle
{
    [Key]
    [Column("id")]
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Marca do veículo
    /// </summary>
    [Column("brand")]
    public string Brand { get; private set; }
    
    /// <summary>
    /// Modelo do veículo
    /// </summary>
    [Column("model")]
    public string Model { get; private set; }
    
    /// <summary>
    /// Ano do veículo
    /// </summary>
    [Column("year")]
    public int Year { get; private set; }
    
    /// <summary>
    /// Cor do veículo
    /// </summary>
    [Column("color")]
    public string Color { get; private set; }
    
    /// <summary>
    /// Preço do veículo
    /// </summary>
    [Column("price")]
    public decimal Price { get; private set; }
    
    /// <summary>
    /// Indica se o veículo foi vendido
    /// </summary>
    [Column("is_sold")]
    public bool IsSold { get; private set; }
    
    /// <summary>
    /// Data de criação do registro
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; private set; }
    
    /// <summary>
    /// Identificador do comprador (CPF)
    /// </summary>
    [Column("buyer_cpf")]
    public string? BuyerCpf { get; private set; }
    
    /// <summary>
    /// Data da venda do veículo
    /// </summary>
    [Column("sale_date")]
    public DateTime? SaleDate { get; private set; }
    
    /// <summary>
    /// Código de pagamento
    /// </summary>
    [Column("payment_code")]
    public string? PaymentCode { get; private set; }

    /// <summary>
    /// Status do pagamento
    /// </summary>
    [Column("payment_status")]
    public PaymentStatus PaymentStatus { get; set; }

    protected Vehicle()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.Pending;
    }

    public Vehicle(string brand, string model, int year, string color, decimal price, bool isSold = false)
    {
        Id = Guid.NewGuid();
        Brand = brand;
        Model = model;
        Year = year;
        Color = color;
        Price = price;
        IsSold = isSold;
        CreatedAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.Pending;
    }

    public void UpdateDetails(string brand, string model, int year, string color, decimal price)
    {
        Brand = brand;
        Model = model;
        Year = year;
        Color = color;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterSale(string buyerCpf, string paymentCode)
    {
        // validações de argumentos
        if (string.IsNullOrWhiteSpace(buyerCpf))
            throw new ArgumentException("CPF do comprador é obrigatório", nameof(buyerCpf));
        
        if (string.IsNullOrWhiteSpace(paymentCode))
            throw new ArgumentException("Código de pagamento é obrigatório", nameof(paymentCode));

        if (IsSold)
            throw new InvalidOperationException("Veículo já está vendido");

        BuyerCpf = buyerCpf;
        SaleDate = DateTime.UtcNow;
        PaymentCode = paymentCode;
        PaymentStatus = PaymentStatus.Pending;
        IsSold = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePaymentStatus(string paymentCode, PaymentStatus status)
    {
        if (!IsSold)
            throw new InvalidOperationException("Veículo não está vendido");

        PaymentStatus = status;
        PaymentCode = paymentCode;
        
        // Se o pagamento foi cancelado, reverter a venda
        if (status == PaymentStatus.Cancelled)
        {
            IsSold = false;
            BuyerCpf = null;
            SaleDate = null;
            PaymentCode = null;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }
}