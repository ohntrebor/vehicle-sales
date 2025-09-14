namespace VehicleSales.Application.DTOs;

public class VehicleSaleDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string BuyerCpf { get; set; }
    public string BuyerName { get; set; }
    public string BuyerEmail { get; set; }
    public decimal SalePrice { get; set; }
    public string PaymentCode { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public VehicleSnapshotDto VehicleData { get; set; }
}