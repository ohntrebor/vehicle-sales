
namespace VehicleSales.Domain.Interfaces;

/// <summary>
/// Interface para Unit of Work pattern
/// </summary>
public interface IUnitOfWork
{
    IVehicleRepository Vehicles { get; }
    Task<int> SaveChangesAsync();
}
