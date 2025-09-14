using VehicleSales.Application.Gateways;

namespace VehicleSales.Application.UseCases;

public class UpdatePaymentStatusUseCase(IVehicleGateway gateway)
{
    public async Task<bool> ExecuteAsync(Guid vehicleId, string paymentCode, string status)
    {
        var vehicle = await gateway.FindByIdAsync(vehicleId);
        if (vehicle == null) return false;

        if (Enum.TryParse<Domain.Enums.PaymentStatus>(status, out var paymentStatus))
        {
            vehicle.UpdatePaymentStatus(paymentCode, paymentStatus);
            await gateway.UpdateAsync(vehicle);
            return true;
        }

        return false;
    }
}