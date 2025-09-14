using VehicleSales.Application.DTOs;
using VehicleSales.Domain.Entities;

namespace VehicleSales.Application.Presenters;

public class SalePresenter : ISalePresenter
{
    public VehicleSaleDto PresentSale(VehicleSale sale)
    {
        return new VehicleSaleDto
        {
            Id = sale.Id,
            VehicleId = sale.VehicleId,
            BuyerCpf = sale.BuyerCpf,
            BuyerName = sale.BuyerName,
            BuyerEmail = sale.BuyerEmail,
            SalePrice = sale.SalePrice,
            PaymentCode = sale.PaymentCode,
            PaymentStatus = sale.PaymentStatus.ToString(),
            CreatedAt = sale.CreatedAt,
            VehicleData = new VehicleSnapshotDto
            {
                Brand = sale.VehicleData.Brand,
                Model = sale.VehicleData.Model,
                Year = sale.VehicleData.Year,
                Color = sale.VehicleData.Color,
                OriginalPrice = sale.VehicleData.OriginalPrice
            }
        };
    }

    public IEnumerable<VehicleSaleDto> PresentSaleList(IEnumerable<VehicleSale> sales)
    {
        return sales.Select(PresentSale);
    }
}