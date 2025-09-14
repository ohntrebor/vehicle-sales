namespace VehicleSales.Application.DTOs;

/// <summary>
/// DTO para retorno de dados do catálogo de veículos (baseado no VehicleDto do Catalog)
/// </summary>
public class VehicleCatalogDto
{
    /// <summary>
    /// ID único do veículo
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Marca do veículo
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Modelo do veículo
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Ano de fabricação
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Cor do veículo
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Preço do veículo
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Status de disponibilidade (se já foi vendido)
    /// </summary>
    public bool IsSold { get; set; }

    /// <summary>
    /// Data de criação no catálogo
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data de última atualização
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}