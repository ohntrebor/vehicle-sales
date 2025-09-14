using Microsoft.AspNetCore.Mvc;
using VehicleSales.Application.Controllers;
using VehicleSales.Application.DTOs;

namespace VehicleSales.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de vendas de veículos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SalesController(SaleUseCaseController useCaseController) : ControllerBase
{
    /// <summary>
    /// Registra uma nova venda de veículo
    /// </summary>
    /// <param name="dto">Dados da venda</param>
    /// <returns>Venda registrada com código de pagamento</returns>
    /// <response code="201">Venda criada com sucesso</response>
    /// <response code="400">Dados inválidos ou veículo não encontrado</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleSaleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto dto)
    {
        try
        {
            var result = await useCaseController.CreateSale(dto);
            return CreatedAtAction(nameof(GetSaleById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Busca uma venda específica por ID
    /// </summary>
    /// <param name="id">ID da venda</param>
    /// <returns>Dados da venda</returns>
    /// <response code="200">Venda encontrada</response>
    /// <response code="404">Venda não encontrada</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VehicleSaleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var result = await useCaseController.GetSaleById(id);
        
        if (result == null)
            return NotFound(new { message = "Venda não encontrada" });

        return Ok(result);
    }

    /// <summary>
    /// Lista todas as vendas registradas
    /// </summary>
    /// <returns>Lista de vendas ordenadas por data</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VehicleSaleDto>), 200)]
    public async Task<IActionResult> GetAllSales()
    {
        var result = await useCaseController.GetAllSales();
        return Ok(result);
    }

    /// <summary>
    /// Webhook para processamento de pagamentos
    /// </summary>
    /// <param name="dto">Dados do webhook de pagamento</param>
    /// <returns>Status do processamento</returns>
    /// <response code="200">Pagamento processado com sucesso</response>
    /// <response code="400">Dados do webhook inválidos</response>
    /// <response code="404">Código de pagamento não encontrado</response>
    [HttpPost("payment-webhook")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PaymentCode))
            return BadRequest(new { message = "Payment code is required" });

        if (string.IsNullOrWhiteSpace(dto.Status))
            return BadRequest(new { message = "Payment status is required" });

        var success = await useCaseController.ProcessPayment(dto);
        
        if (success)
            return Ok(new { message = "Payment processed successfully" });
        
        return NotFound(new { message = "Payment code not found" });
    }
}