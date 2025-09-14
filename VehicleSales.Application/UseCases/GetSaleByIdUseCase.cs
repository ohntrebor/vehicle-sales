using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.UseCases;

public class GetSaleByIdUseCase(ISaleGateway gateway)
{
    public async Task<VehicleSale?> ExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Sale ID cannot be empty");

        return await gateway.FindByIdAsync(id);
    }
}