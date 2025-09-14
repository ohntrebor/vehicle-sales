using VehicleSales.Application.DTOs;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Presenters;

public interface ISalePresenter
{
    VehicleSaleDto PresentSale(VehicleSale sale);
    IEnumerable<VehicleSaleDto> PresentSaleList(IEnumerable<VehicleSale> sales);
}