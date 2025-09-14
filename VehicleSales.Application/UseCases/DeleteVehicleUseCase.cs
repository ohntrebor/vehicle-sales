using VehicleSales.Application.Gateways;

namespace VehicleSales.Application.UseCases;

public class DeleteVehicleUseCase(IVehicleGateway gateway)
{
    public async Task<bool> ExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Vehicle ID cannot be empty");

        return await gateway.DeleteAsync(id);
    }
}