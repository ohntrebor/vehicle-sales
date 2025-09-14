using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.UseCases;

public class UpdateVehicleUseCase(IVehicleGateway gateway)
{
    public async Task<Vehicle> ExecuteAsync(Guid id, string brand, string model, int year, string color, decimal price)
    {
        var vehicle = await gateway.FindByIdAsync(id);
        if (vehicle == null) throw new ArgumentException("Vehicle not found");

        // Validações
        if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required");
        if (price <= 0) throw new ArgumentException("Price must be greater than 0");

        vehicle.UpdateDetails(brand, model, year, color, price);
        return await gateway.UpdateAsync(vehicle);
    }
}