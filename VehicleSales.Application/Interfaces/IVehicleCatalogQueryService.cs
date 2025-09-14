using VehicleSales.Application.DTOs;

namespace VehicleSales.Application.Interfaces;

/// <summary>
/// Interface para consultas no catálogo de veículos (camada Application)
/// </summary>
public interface IVehicleCatalogQueryService
{
    /// <summary>
    /// Busca um veículo específico por ID com dados completos do catálogo
    /// </summary>
    /// <param name="vehicleId">ID do veículo</param>
    /// <returns>Dados completos do veículo conforme disponível no catálogo</returns>
    Task<VehicleCatalogDto?> GetVehicleByIdAsync(Guid vehicleId);

    /// <summary>
    /// Lista todos os veículos disponíveis no catálogo
    /// </summary>
    /// <returns>Lista de veículos disponíveis para venda</returns>
    Task<IEnumerable<VehicleCatalogDto>?> GetAllVehiclesAsync();

    /// <summary>
    /// Pesquisa veículos disponíveis por filtros específicos
    /// </summary>
    /// <param name="brand">Marca do veículo (opcional)</param>
    /// <param name="minPrice">Preço mínimo (opcional)</param>
    /// <param name="maxPrice">Preço máximo (opcional)</param>
    /// <param name="year">Ano de fabricação (opcional)</param>
    /// <returns>Lista filtrada de veículos disponíveis</returns>
    Task<IEnumerable<VehicleCatalogDto>?> SearchVehiclesAsync(
        string? brand = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        int? year = null);
}