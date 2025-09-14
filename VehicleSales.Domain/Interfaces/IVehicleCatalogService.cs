using VehicleSales.Domain.Entities;

namespace VehicleSales.Domain.Interfaces;

public interface IVehicleCatalogService
{
    Task<VehicleSnapshot?> GetVehicleAsync(Guid vehicleId);
    Task<bool> NotifyVehicleSoldAsync(Guid vehicleId, string paymentCode, string status);
}