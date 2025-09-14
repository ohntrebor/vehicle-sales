using VehicleSales.Domain.Entities;

namespace VehicleSales.Domain.Interfaces;

public interface IVehicleSaleRepository
{
    Task<VehicleSale> CreateAsync(VehicleSale sale);
    Task<VehicleSale?> GetByIdAsync(Guid id);
    Task<VehicleSale?> GetByPaymentCodeAsync(string paymentCode);
    Task<IEnumerable<VehicleSale>> GetByBuyerCpfAsync(string cpf);
    Task<IEnumerable<VehicleSale>> GetAllAsync();
    Task UpdateAsync(VehicleSale sale);
}