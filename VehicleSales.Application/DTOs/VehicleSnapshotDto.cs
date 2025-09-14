namespace VehicleSales.Application.DTOs;

public class VehicleSnapshotDto
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public decimal OriginalPrice { get; set; }
}