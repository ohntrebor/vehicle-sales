using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Tests.Mocks;

/// <summary>
/// Mock do VehicleCatalogService para isolamento nos testes
/// </summary>
public class MockVehicleCatalogService : IVehicleCatalogService
{
    private readonly Dictionary<Guid, VehicleSnapshot> _mockVehicles = new()
    {
        // Veículos para testes específicos
        {
            new Guid("11111111-1111-1111-1111-111111111111"),
            new VehicleSnapshot
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2022,
                Color = "Prata",
                OriginalPrice = 85000.00m
            }
        },
        {
            new Guid("22222222-2222-2222-2222-222222222222"),
            new VehicleSnapshot
            {
                Brand = "BMW",
                Model = "X1",
                Year = 2023,
                Color = "Branco",
                OriginalPrice = 180000.00m
            }
        },
        {
            new Guid("33333333-3333-3333-3333-333333333333"),
            new VehicleSnapshot
            {
                Brand = "Honda",
                Model = "Civic",
                Year = 2022,
                Color = "Azul",
                OriginalPrice = 95000.00m
            }
        },
        {
            new Guid("44444444-4444-4444-4444-444444444444"),
            new VehicleSnapshot
            {
                Brand = "Ford",
                Model = "Ka",
                Year = 2021,
                Color = "Vermelho",
                OriginalPrice = 45000.00m
            }
        }
    };
    private readonly List<(Guid VehicleId, string PaymentCode, string Status)> _notificationHistory = new();

    // Dados mockados para testes
    // Veículos para testes específicos

    /// <summary>
    /// Simula busca de veículo no catálogo
    /// </summary>
    public Task<VehicleSnapshot?> GetVehicleAsync(Guid vehicleId)
    {
        // Se encontrar o veículo mockado, retorna
        if (_mockVehicles.TryGetValue(vehicleId, out var vehicle))
        {
            return Task.FromResult<VehicleSnapshot?>(vehicle);
        }

        // Se não encontrar, retorna um veículo genérico (para testes flexíveis)
        var genericVehicle = new VehicleSnapshot
        {
            Brand = "Generic",
            Model = "TestModel",
            Year = 2022,
            Color = "Branco",
            OriginalPrice = 50000.00m
        };

        return Task.FromResult<VehicleSnapshot?>(genericVehicle);
    }

    /// <summary>
    /// Simula notificação para o VehicleCatalog
    /// </summary>
    public Task<bool> NotifyVehicleSoldAsync(Guid vehicleId, string paymentCode, string status)
    {
        // Registra a notificação para verificação nos testes
        _notificationHistory.Add((vehicleId, paymentCode, status));

        // Simula diferentes cenários baseado no status
        return status.ToUpper() switch
        {
            "PAID" => Task.FromResult(true),      // Sempre sucesso para pagamento aprovado
            "CANCELLED" => Task.FromResult(true), // Sempre sucesso para cancelamento
            "FAILED" => Task.FromResult(false),   // Simula falha
            _ => Task.FromResult(true)            // Default: sucesso
        };
    }

    // ==============================================
    // MÉTODOS AUXILIARES PARA TESTES
    // ==============================================

    /// <summary>
    /// Adiciona um veículo mockado para testes específicos
    /// </summary>
    public void AddMockVehicle(Guid vehicleId, VehicleSnapshot vehicle)
    {
        _mockVehicles[vehicleId] = vehicle;
    }

    /// <summary>
    /// Remove um veículo mockado (simula veículo não encontrado)
    /// </summary>
    public void RemoveMockVehicle(Guid vehicleId)
    {
        _mockVehicles.Remove(vehicleId);
    }

    /// <summary>
    /// Verifica se uma notificação foi enviada
    /// </summary>
    public bool WasNotificationSent(Guid vehicleId, string paymentCode, string status)
    {
        return _notificationHistory.Any(n => 
            n.VehicleId == vehicleId && 
            n.PaymentCode == paymentCode && 
            n.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Retorna histórico de notificações para verificação nos testes
    /// </summary>
    public IReadOnlyList<(Guid VehicleId, string PaymentCode, string Status)> GetNotificationHistory()
    {
        return _notificationHistory.AsReadOnly();
    }

    /// <summary>
    /// Limpa histórico de notificações
    /// </summary>
    public void ClearNotificationHistory()
    {
        _notificationHistory.Clear();
    }

    /// <summary>
    /// Simula erro de conexão
    /// </summary>
    public void SimulateConnectionError(bool shouldFail = true)
    {
        // Para testes mais avançados, você pode adicionar lógica para simular falhas
        // Por exemplo, lançar exceções ou retornar false
    }
}

// ==============================================
// EXTENSÕES PARA FACILITAR TESTES
// ==============================================
public static class MockVehicleCatalogServiceExtensions
{
    /// <summary>
    /// Configura um cenário de teste com veículos específicos
    /// </summary>
    public static MockVehicleCatalogService WithTestVehicles(this MockVehicleCatalogService mock)
    {
        var testVehicles = new[]
        {
            (new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Volkswagen", "Polo", 65000.00m),
            (new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Fiat", "Uno", 35000.00m),
            (new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Nissan", "Sentra", 80000.00m)
        };

        foreach (var (id, brand, model, price) in testVehicles)
        {
            mock.AddMockVehicle(id, new VehicleSnapshot
            {
                Brand = brand,
                Model = model,
                Year = 2022,
                Color = "Test",
                OriginalPrice = price
            });
        }

        return mock;
    }
}