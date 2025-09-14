using VehicleSales.Domain.Entities;

namespace VehicleSales.Domain.Interfaces;

/// <summary>
/// Interface para serviço de catálogo de veículos
/// </summary>
public interface IVehicleCatalogService
{
    /// <summary>
    /// Busca um veículo específico por ID para criação de snapshot de venda
    /// </summary>
    /// <param name="vehicleId">ID do veículo</param>
    /// <returns>Snapshot do veículo para uso interno na venda</returns>
    Task<VehicleSnapshot?> GetVehicleAsync(Guid vehicleId);

    /// <summary>
    /// Notifica o catálogo sobre mudança no status de venda de um veículo
    /// </summary>
    /// <param name="vehicleId">ID do veículo vendido</param>
    /// <param name="paymentCode">Código de pagamento gerado</param>
    /// <param name="status">Status do pagamento (pending, approved, declined)</param>
    /// <returns>True se a notificação foi enviada com sucesso</returns>
    Task<bool> NotifyVehicleSoldAsync(Guid vehicleId, string paymentCode, string status);
}