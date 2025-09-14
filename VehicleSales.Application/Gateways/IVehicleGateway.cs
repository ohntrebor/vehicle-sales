using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Gateways;

public interface IVehicleGateway
{
    Task<Vehicle> SaveAsync(Vehicle vehicle);
    Task<Vehicle> UpdateAsync(Vehicle vehicle);
    Task<Vehicle?> FindByIdAsync(Guid id);
    Task<IEnumerable<Vehicle>> FindAvailableVehiclesAsync();
    Task<IEnumerable<Vehicle>> FindSoldVehiclesAsync();
    Task<bool> DeleteAsync(Guid id);

}