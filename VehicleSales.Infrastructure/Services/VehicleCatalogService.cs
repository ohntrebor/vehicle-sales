using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using VehicleSales.Application.DTOs;
using VehicleSales.Application.Interfaces;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de catálogo de veículos
/// Implementa interfaces tanto do Domain quanto da Application
/// </summary>
public class VehicleCatalogService(HttpClient httpClient, ILogger<VehicleCatalogService> logger)
    : IVehicleCatalogService, IVehicleCatalogQueryService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // Configuração automática da BaseAddress no momento da criação
    private readonly HttpClient _httpClient = ConfigureHttpClient(httpClient, logger);

    private static HttpClient ConfigureHttpClient(HttpClient httpClient, ILogger logger)
    {
        if (httpClient.BaseAddress == null)
        {
            // 1. Variável de ambiente (configurada no docker-compose)
            // 2. Fallback para desenvolvimento local
            var baseUrl = Environment.GetEnvironmentVariable("ExternalServices__VehicleCatalogApi") 
                          ?? "http://localhost:5000";
            
            httpClient.BaseAddress = new Uri(baseUrl);
            logger.LogInformation("BaseAddress configurada via fallback: {BaseAddress}", httpClient.BaseAddress);
        }
        else
        {
            logger.LogInformation("BaseAddress configurada via HttpClient: {BaseAddress}", httpClient.BaseAddress);
        }
        return httpClient;
    }

    #region Domain Interface Implementation (IVehicleCatalogService)

    /// <summary>
    /// Busca um veículo específico por ID para criação de snapshot de venda
    /// </summary>
    public async Task<VehicleSnapshot?> GetVehicleAsync(Guid vehicleId)
    {
        try
        {
            logger.LogInformation("Buscando veículo {VehicleId} no catálogo para snapshot...", vehicleId);
            
            var vehicle = await GetVehicleByIdAsync(vehicleId);
            
            if (vehicle == null)
            {
                logger.LogWarning("Veículo {VehicleId} não encontrado ou não está disponível", vehicleId);
                return null;
            }

            return new VehicleSnapshot
            {
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Color = vehicle.Color,
                OriginalPrice = vehicle.Price
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar veículo {VehicleId} para snapshot", vehicleId);
            return null;
        }
    }

    /// <summary>
    /// Notifica o catálogo sobre mudança no status de venda de um veículo
    /// </summary>
    public async Task<bool> NotifyVehicleSoldAsync(Guid vehicleId, string paymentCode, string status)
    {
        try
        {
            logger.LogInformation("Notificando venda do veículo {VehicleId} com status {Status}...", vehicleId, status);

            var payload = new
            {
                VehicleId = vehicleId,
                PaymentCode = paymentCode,
                Status = status
            };

            var json = JsonSerializer.Serialize(payload, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/vehicles/payment-webhook", content);
            
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Webhook enviado com sucesso para veículo {VehicleId}", vehicleId);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogWarning("Erro ao enviar webhook: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao notificar venda do veículo {VehicleId}", vehicleId);
            return false;
        }
    }

    #endregion

    #region Application Interface Implementation (IVehicleCatalogQueryService)

    /// <summary>
    /// Busca um veículo específico por ID com dados completos do catálogo
    /// </summary>
    public async Task<VehicleCatalogDto?> GetVehicleByIdAsync(Guid vehicleId)
    {
        try
        {
            logger.LogInformation("Buscando veículo específico {VehicleId}...", vehicleId);
            
            var response = await _httpClient.GetAsync($"/api/vehicles/{vehicleId}");
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogWarning("Veículo {VehicleId} não encontrado", vehicleId);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Erro ao buscar veículo {VehicleId}: {StatusCode}", vehicleId, response.StatusCode);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vehicle = JsonSerializer.Deserialize<VehicleCatalogDto>(jsonContent, _jsonOptions);

            return vehicle;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar veículo específico {VehicleId}", vehicleId);
            return null;
        }
    }

    /// <summary>
    /// Lista todos os veículos disponíveis no catálogo
    /// </summary>
    public async Task<IEnumerable<VehicleCatalogDto>?> GetAllVehiclesAsync()
    {
        try
        {
            logger.LogInformation("Buscando todos os veículos disponíveis...");
            
            var response = await _httpClient.GetAsync("api/vehicles/available");
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Erro ao buscar veículos disponíveis: {StatusCode}", response.StatusCode);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vehicles = JsonSerializer.Deserialize<VehicleCatalogDto[]>(jsonContent, _jsonOptions);

            logger.LogInformation("Encontrados {Count} veículos disponíveis", vehicles?.Length ?? 0);
            return vehicles ?? Enumerable.Empty<VehicleCatalogDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao buscar todos os veículos");
            return null;
        }
    }

    /// <summary>
    /// Pesquisa veículos disponíveis por filtros específicos
    /// </summary>
    public async Task<IEnumerable<VehicleCatalogDto>?> SearchVehiclesAsync(
        string? brand = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        int? year = null)
    {
        try
        {
            logger.LogInformation("Pesquisando veículos com filtros: Brand={Brand}, MinPrice={MinPrice}, MaxPrice={MaxPrice}, Year={Year}",
                brand, minPrice, maxPrice, year);
            
            var queryParams = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(brand))
                queryParams.Add($"brand={Uri.EscapeDataString(brand)}");
            
            if (minPrice.HasValue)
                queryParams.Add($"minPrice={minPrice.Value}");
            
            if (maxPrice.HasValue)
                queryParams.Add($"maxPrice={maxPrice.Value}");
            
            if (year.HasValue)
                queryParams.Add($"year={year.Value}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"/api/vehicles/search{queryString}");
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Erro ao pesquisar veículos: {StatusCode}", response.StatusCode);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vehicles = JsonSerializer.Deserialize<VehicleCatalogDto[]>(jsonContent, _jsonOptions);

            logger.LogInformation("Pesquisa retornou {Count} veículos", vehicles?.Length ?? 0);
            return vehicles ?? Enumerable.Empty<VehicleCatalogDto>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao pesquisar veículos");
            return null;
        }
    }

    #endregion
}