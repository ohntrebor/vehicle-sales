using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Gateways;

public interface ISaleGateway
{
    Task<VehicleSale> CreateSaleAsync(VehicleSale sale);
    Task<VehicleSale?> FindByIdAsync(Guid id);
    Task<VehicleSale?> FindByPaymentCodeAsync(string paymentCode);
    Task<IEnumerable<VehicleSale>> FindAllAsync();
    Task<VehicleSale> UpdateSaleAsync(VehicleSale sale);
}