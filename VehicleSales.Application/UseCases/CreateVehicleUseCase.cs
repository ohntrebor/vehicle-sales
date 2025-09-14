using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.UseCases;

public class CreateVehicleUseCase(IVehicleGateway gateway)
{
    public async Task<Vehicle> ExecuteAsync(string brand, string model, int year, string color, decimal price)
    {
        // Validações simples
        if (string.IsNullOrWhiteSpace(brand)) throw new ArgumentException("Brand is required");
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Model is required");
        if (year < 1900 || year > 2030) throw new ArgumentException("Invalid year");
        if (price <= 0) throw new ArgumentException("Price must be greater than 0");

        var vehicle = new Vehicle(brand, model, year, color, price);
        return await gateway.SaveAsync(vehicle);
    }
}