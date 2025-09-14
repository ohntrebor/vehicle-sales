using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Enums;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Application.UseCases;

public class ProcessPaymentUseCase(ISaleGateway gateway, IVehicleCatalogService catalogService)
{
    public async Task<bool> ExecuteAsync(string paymentCode, string status)
    {
        var sale = await gateway.FindByPaymentCodeAsync(paymentCode);
        if (sale == null) return false;

        // Atualizar status local
        if (Enum.TryParse<PaymentStatus>(status, out var paymentStatus))
        {
            sale.UpdatePaymentStatus(paymentStatus);
            await gateway.UpdateSaleAsync(sale);

            // Se pagamento aprovado, notificar o serviço de catálogo
            if (paymentStatus == PaymentStatus.Paid)
            {
                await catalogService.NotifyVehicleSoldAsync(sale.VehicleId, paymentCode, status);
            }

            return true;
        }

        return false;
    }
}