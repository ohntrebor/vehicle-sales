using VehicleSales.Domain.Entities;

namespace VehicleSales.Domain.Interfaces;

/// <summary>
/// Interface do repositório de veículos
/// </summary>
public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle?> GetByPaymentCodeAsync(Guid vehicleId);
    Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
    Task<IEnumerable<Vehicle>> GetSoldVehiclesAsync();
    Task<Vehicle> AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(Vehicle vehicle);

    
}
