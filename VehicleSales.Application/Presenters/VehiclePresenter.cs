using VehicleSales.Application.DTOs;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Presenters;

public class VehiclePresenter : IVehiclePresenter
{
    public VehicleDto PresentVehicle(Vehicle vehicle)
    {
        return new VehicleDto
        {
            Id = vehicle.Id,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Price = vehicle.Price
        };
    }

    public IEnumerable<VehicleDto> PresentVehicleList(IEnumerable<Vehicle> vehicles)
    {
        return vehicles.Select(PresentVehicle);
    }

    public VehicleSaleDto PresentSoldVehicle(Vehicle vehicle)
    {
        return new VehicleSaleDto
        {
            Id = vehicle.Id,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Price = vehicle.Price,
            PaymentStatus = vehicle.PaymentStatus.ToString(),
            PaymentCode = vehicle.PaymentCode,
            SaleDate = vehicle.SaleDate,
            IsSold = vehicle.IsSold
        };
    }

    public IEnumerable<VehicleSaleDto> PresentSoldVehicleList(IEnumerable<Vehicle> vehicles)
    {
        return vehicles.Select(PresentSoldVehicle);
    }
}