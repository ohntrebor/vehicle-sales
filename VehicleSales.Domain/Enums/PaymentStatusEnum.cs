using System.ComponentModel;

namespace VehicleSales.Domain.Enums;

/// <summary>
/// Representa os diferentes estados de uma transação de pagamento no sistema de vendas de veículos.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Pagamento aguardando processamento ou ação do cliente.
    /// </summary>
    [Description("Aguardando")]
    Pending = 0,
    
    /// <summary>
    /// Pagamento foi processado com sucesso e confirmado.
    /// </summary>
    [Description("Pago")]
    Paid = 1,
    
    /// <summary>
    /// Pagamento foi cancelado pelo cliente ou sistema antes da conclusão.
    /// </summary>
    [Description("Cancelado")]
    Cancelled = 2,
    
    /// <summary>
    /// Falha no processamento do pagamento devido a fundos insuficientes, problemas técnicos ou outros erros.
    /// </summary>
    [Description("Falhou")]
    Failed = 3
}