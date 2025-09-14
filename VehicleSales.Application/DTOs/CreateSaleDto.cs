namespace VehicleSales.Application.DTOs;

public class CreateSaleDto
{
    public Guid VehicleId { get; set; }
    public string BuyerCpf { get; set; }
    public string BuyerName { get; set; }
    public string BuyerEmail { get; set; }
}