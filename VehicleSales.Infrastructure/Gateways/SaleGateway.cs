using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Infrastructure.Gateways;

public class SaleGateway(IVehicleSaleRepository repository) : ISaleGateway
{
    public async Task<VehicleSale> CreateSaleAsync(VehicleSale sale)
    {
        return await repository.CreateAsync(sale);
    }

    public async Task<VehicleSale?> FindByIdAsync(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<VehicleSale?> FindByPaymentCodeAsync(string paymentCode)
    {
        return await repository.GetByPaymentCodeAsync(paymentCode);
    }

    public async Task<IEnumerable<VehicleSale>> FindAllAsync()
    {
        return await repository.GetAllAsync();
    }

    public async Task<VehicleSale> UpdateSaleAsync(VehicleSale sale)
    {
        await repository.UpdateAsync(sale);
        return sale;
    }
}