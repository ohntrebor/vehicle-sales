using System.Text.Json;
using Microsoft.Extensions.Logging;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Infrastructure.Services;

public class VehicleCatalogService(HttpClient httpClient, ILogger<VehicleCatalogService> logger)
    : IVehicleCatalogService
{
    public async Task<VehicleSnapshot?> GetVehicleAsync(Guid vehicleId)
    {
        try
        {
            logger.LogInformation($"Buscando veículo {vehicleId} no catálogo...");
            
            var response = await httpClient.GetAsync($"/vehicles/available");
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning($"Erro ao buscar veículos: {response.StatusCode}");
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vehicles = JsonSerializer.Deserialize<VehicleCatalogDto[]>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var vehicle = vehicles?.FirstOrDefault(v => v.Id == vehicleId);
            
            if (vehicle == null)
            {
                logger.LogWarning($"Veículo {vehicleId} não encontrado ou não está disponível");
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
            logger.LogError(ex, $"Erro ao buscar veículo {vehicleId}");
            return null;
        }
    }

    public async Task<bool> NotifyVehicleSoldAsync(Guid vehicleId, string paymentCode, string status)
    {
        try
        {
            logger.LogInformation($"Notificando venda do veículo {vehicleId} com status {status}...");

            var payload = new
            {
                VehicleId = vehicleId,
                PaymentCode = paymentCode,
                Status = status
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/vehicles/payment-webhook", content);
            
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation($"Webhook enviado com sucesso para veículo {vehicleId}");
                return true;
            }
            else
            {
                logger.LogWarning($"Erro ao enviar webhook: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Erro ao notificar venda do veículo {vehicleId}");
            return false;
        }
    }
}

// DTO para deserializar resposta da API VehicleCatalog
internal class VehicleCatalogDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
