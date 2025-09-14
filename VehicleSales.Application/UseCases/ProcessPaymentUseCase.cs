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

        // Mapear status recebido para PaymentStatus
        var paymentStatus = MapStringToPaymentStatus(status);
        if (paymentStatus == null) return false;

        sale.UpdatePaymentStatus(paymentStatus.Value);
        await gateway.UpdateSaleAsync(sale);

        // Se pagamento aprovado, notificar o serviço de catálogo
        if (paymentStatus == PaymentStatus.Paid)
        {
            await catalogService.NotifyVehicleSoldAsync(sale.VehicleId, paymentCode, status);
        }

        return true;
    }

    private PaymentStatus? MapStringToPaymentStatus(string status)
    {
        return status?.ToLowerInvariant() switch
        {
           "0" or "pending" or "processing" => PaymentStatus.Pending,
           "1" or "confirmed" or "paid" or "approved" => PaymentStatus.Paid,
           "2" or"cancelled" or "canceled" => PaymentStatus.Cancelled,
           "3" or "failed" or "rejected" or "declined" => PaymentStatus.Failed,
            _ => null
        };
    }
}