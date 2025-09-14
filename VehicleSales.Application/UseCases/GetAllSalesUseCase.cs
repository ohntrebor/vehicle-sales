using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.UseCases;

public class GetAllSalesUseCase(ISaleGateway gateway)
{
    public async Task<IEnumerable<VehicleSale>> ExecuteAsync()
    {
        return await gateway.FindAllAsync();
    }
}