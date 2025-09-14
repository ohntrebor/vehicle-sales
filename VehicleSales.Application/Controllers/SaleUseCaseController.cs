using VehicleSales.Application.DTOs;
using VehicleSales.Application.Gateways;
using VehicleSales.Application.Presenters;
using VehicleSales.Application.UseCases;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Application.Controllers;

public class SaleUseCaseController(
    ISaleGateway gateway,
    ISalePresenter presenter,
    IVehicleCatalogService catalogService)
{
    public async Task<VehicleSaleDto> CreateSale(CreateSaleDto dto)
    {
        var useCase = new CreateSaleUseCase(gateway, catalogService);
        var sale = await useCase.ExecuteAsync(dto.VehicleId, dto.BuyerCpf, dto.BuyerName, dto.BuyerEmail);
        return presenter.PresentSale(sale);
    }

    public async Task<VehicleSaleDto?> GetSaleById(Guid id)
    {
        var useCase = new GetSaleByIdUseCase(gateway);
        var sale = await useCase.ExecuteAsync(id);
        return sale != null ? presenter.PresentSale(sale) : null;
    }

    public async Task<IEnumerable<VehicleSaleDto>> GetAllSales()
    {
        var useCase = new GetAllSalesUseCase(gateway);
        var sales = await useCase.ExecuteAsync();
        return presenter.PresentSaleList(sales);
    }

    public async Task<bool> ProcessPayment(PaymentWebhookDto dto)
    {
        var useCase = new ProcessPaymentUseCase(gateway, catalogService);
        return await useCase.ExecuteAsync(dto.PaymentCode, dto.Status);
    }
}