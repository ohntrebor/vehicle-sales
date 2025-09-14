using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.UseCases;

public class GetSoldVehiclesUseCase(IVehicleGateway gateway)
{
    public async Task<IEnumerable<Vehicle>> ExecuteAsync()
    {
        return await gateway.FindSoldVehiclesAsync();
    }
}