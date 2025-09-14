using System.ComponentModel;

namespace VehicleSales.Domain.Enums;

public enum PaymentStatus
{
    /// <summary>
    /// Pendente
    /// </summary>
    [Description("Pendente")]
    Pending = 0,
    
    /// <summary>
    /// Confirmado
    /// </summary>
    [Description("Confirmado")]
    Confirmed = 1,
    
    /// <summary>
    /// Cancelado
    /// </summary>
    [Description("Cancelado")]
    Cancelled = 2
}