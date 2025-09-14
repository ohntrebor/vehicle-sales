using VehicleSales.Application.Gateways;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Application.UseCases;

public class CreateSaleUseCase(ISaleGateway gateway, IVehicleCatalogService catalogService)
{
    private readonly ISaleGateway _gateway = gateway;

    public async Task<VehicleSale> ExecuteAsync(Guid vehicleId, string buyerCpf, string buyerName, string buyerEmail)
    {
        // Buscar dados do veículo no serviço de catálogo
        var vehicleData = await catalogService.GetVehicleAsync(vehicleId);
        if (vehicleData == null)
            throw new ArgumentException("Vehicle not found");

        // Validações simples
        if (string.IsNullOrWhiteSpace(buyerCpf))
            throw new ArgumentException("Buyer CPF is required");

        if (string.IsNullOrWhiteSpace(buyerName))
            throw new ArgumentException("Buyer name is required");

        // Criar venda
        var sale = new VehicleSale(vehicleId, buyerCpf, buyerName, buyerEmail, vehicleData.OriginalPrice, vehicleData);
        
        return await _gateway.CreateSaleAsync(sale);
    }
}