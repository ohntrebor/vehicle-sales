using VehicleSales.Application.DTOs;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Presenters;

public interface IVehiclePresenter
{
    VehicleDto PresentVehicle(Vehicle vehicle);
    IEnumerable<VehicleDto> PresentVehicleList(IEnumerable<Vehicle> vehicles);
    VehicleSaleDto PresentSoldVehicle(Vehicle vehicle);
    IEnumerable<VehicleSaleDto> PresentSoldVehicleList(IEnumerable<Vehicle> vehicles);
}