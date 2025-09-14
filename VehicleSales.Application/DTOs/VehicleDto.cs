
namespace VehicleSales.Application.DTOs;

/// <summary>
/// DTO para representação de veículo
/// </summary>
public class VehicleDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
    public bool IsSold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class VehicleSaleDto : VehicleDto
{
    public string? BuyerCpf { get; set; }
    public DateTime? SaleDate { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentCode { get; set; }
    public DateTime? SoldAt { get; set; }
}

public class CreateVehicleDto
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
}

public class UpdateVehicleDto : CreateVehicleDto
{
    public Guid Id { get; set; }
}

public class PaymentWebhookDto
{
    public Guid VehicleId { get; set; }
    public string PaymentCode { get; set; }
    public string Status { get; set; }
}
