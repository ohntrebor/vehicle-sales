using Microsoft.AspNetCore.Mvc;
using VehicleSales.Application.DTOs;
using VehicleSales.Application.Interfaces;

namespace VehicleSales.API.Controllers;

/// <summary>
/// Controller responsável pela consulta de catálogo de veículos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CatalogController(IVehicleCatalogQueryService catalogQueryService) : ControllerBase
{
    /// <summary>
    /// Lista todos os veículos disponíveis no catálogo
    /// </summary>
    /// <returns>Lista de veículos disponíveis ordenados por preço</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    /// <response code="503">Serviço de catálogo indisponível</response>
    [HttpGet("vehicles")]
    [ProducesResponseType(typeof(IEnumerable<VehicleCatalogDto>), 200)]
    [ProducesResponseType(503)]
    public async Task<IActionResult> GetAllVehicles()
    {
        try
        {
            var vehicles = await catalogQueryService.GetAllVehiclesAsync();
            
            if (vehicles == null)
                return StatusCode(503, new { message = "Serviço de catálogo temporariamente indisponível" });

            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { 
                message = "Erro ao consultar catálogo de veículos",
                details = ex.Message 
            });
        }
    }

   /// <summary>
    /// Busca veículos por filtros específicos
    /// </summary>
    /// <param name="brand">Marca do veículo</param>
    /// <param name="model">Modelo do veículo</param>
    /// <param name="minPrice">Preço mínimo</param>
    /// <param name="maxPrice">Preço máximo</param>
    /// <param name="year">Ano específico</param>
    /// <param name="minYear">Ano mínimo</param>
    /// <param name="maxYear">Ano máximo</param>
    /// <param name="color">Cor do veículo</param>
    /// <param name="isAvailable">Apenas disponíveis (true), apenas vendidos (false) ou todos (null)</param>
    /// <returns>Lista filtrada de veículos</returns>
    /// <response code="200">Lista filtrada retornada com sucesso</response>
    /// <response code="400">Parâmetros de filtro inválidos</response>
    /// <response code="503">Serviço de catálogo indisponível</response>
    [HttpGet("vehicles/search")]
    [ProducesResponseType(typeof(IEnumerable<VehicleCatalogDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(503)]
    public async Task<IActionResult> SearchVehicles(
        [FromQuery] string? brand = null,
        [FromQuery] string? model = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int? year = null,
        [FromQuery] int? minYear = null,
        [FromQuery] int? maxYear = null,
        [FromQuery] string? color = null,
        [FromQuery] bool? isAvailable = null)
    {
        try
        {
            // Validações básicas
            if (minPrice.HasValue && minPrice < 0)
                return BadRequest(new { message = "Preço mínimo não pode ser negativo" });
                
            if (maxPrice.HasValue && maxPrice < 0)
                return BadRequest(new { message = "Preço máximo não pode ser negativo" });
                
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest(new { message = "Preço mínimo não pode ser maior que o máximo" });

            if (minYear.HasValue && maxYear.HasValue && minYear > maxYear)
                return BadRequest(new { message = "Ano mínimo não pode ser maior que o máximo" });

            var vehicles = await catalogQueryService.SearchVehiclesAsync(
                brand, model, minPrice, maxPrice, year, minYear, maxYear, color, isAvailable);
            
            if (vehicles == null)
                return StatusCode(503, new { message = "Serviço de catálogo temporariamente indisponível" });

            return Ok(vehicles);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { 
                message = "Erro ao pesquisar veículos",
                details = ex.Message 
            });
        }
    }
   
    /// <summary>
    /// Busca um veículo específico por ID
    /// </summary>
    /// <param name="id">ID do veículo</param>
    /// <returns>Dados do veículo</returns>
    /// <response code="200">Veículo encontrado</response>
    /// <response code="404">Veículo não encontrado</response>
    /// <response code="503">Serviço de catálogo indisponível</response>
    [HttpGet("vehicles/{id:guid}")]
    [ProducesResponseType(typeof(VehicleCatalogDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(503)]
    public async Task<IActionResult> GetVehicleById(Guid id)
    {
        try
        {
            var vehicle = await catalogQueryService.GetVehicleByIdAsync(id);
            
            if (vehicle == null)
                return NotFound(new { message = "Veículo não encontrado" });

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { 
                message = "Erro ao buscar veículo",
                details = ex.Message 
            });
        }
    }
}