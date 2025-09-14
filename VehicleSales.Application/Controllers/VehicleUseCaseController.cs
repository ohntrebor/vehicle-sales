using VehicleSales.Application.DTOs;
using VehicleSales.Application.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.UseCases;

namespace VehicleSales.Application.Controllers;

public class VehicleUseCaseController(IVehicleGateway gateway, IVehiclePresenter presenter)
{
    public async Task<VehicleDto> CreateVehicle(CreateVehicleDto dto)
    {
        var useCase = new CreateVehicleUseCase(gateway);
        var vehicle = await useCase.ExecuteAsync(dto.Brand, dto.Model, dto.Year, dto.Color, dto.Price);
        return presenter.PresentVehicle(vehicle);
    }

    public async Task<VehicleDto> UpdateVehicle(UpdateVehicleDto dto)
    {
        var useCase = new UpdateVehicleUseCase(gateway);
        var vehicle = await useCase.ExecuteAsync(dto.Id, dto.Brand, dto.Model, dto.Year, dto.Color, dto.Price);
        return presenter.PresentVehicle(vehicle);
    }

    public async Task<IEnumerable<VehicleDto>> GetAvailableVehicles()
    {
        var useCase = new GetAvailableVehiclesUseCase(gateway);
        var vehicles = await useCase.ExecuteAsync();
        return presenter.PresentVehicleList(vehicles);
    }

    public async Task<IEnumerable<VehicleSaleDto>> GetSoldVehicles()
    {
        var useCase = new GetSoldVehiclesUseCase(gateway);
        var vehicles = await useCase.ExecuteAsync();
        return presenter.PresentSoldVehicleList(vehicles);
    }

    public async Task<bool> UpdatePaymentStatus(PaymentWebhookDto dto)
    {
        var useCase = new UpdatePaymentStatusUseCase(gateway);
        return await useCase.ExecuteAsync(dto.VehicleId, dto.PaymentCode, dto.Status);
    }
    
    public async Task<bool> DeleteVehicle(Guid id)
    {
        var useCase = new DeleteVehicleUseCase(gateway);
        return await useCase.ExecuteAsync(id);
    }
}