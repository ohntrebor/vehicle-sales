using Microsoft.AspNetCore.Mvc;
using VehicleSales.Application.Controllers;
using VehicleSales.Application.DTOs;

namespace VehicleSales.API.Controllers;

/// <summary>
/// Controller responsável pelo gerenciamento de veículos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehiclesController(VehicleUseCaseController useCaseController) : ControllerBase
{
    /// <summary>
    /// Cadastra um novo veículo para venda
    /// </summary>
    /// <param name="dto">Dados do veículo a ser cadastrado</param>
    /// <returns>Veículo cadastrado com sucesso</returns>
    /// <response code="201">Veículo criado com sucesso</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    [HttpPost]
    [ProducesResponseType(typeof(VehicleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto dto)
    {
        try
        {
            var result = await useCaseController.CreateVehicle(dto);
            return CreatedAtAction(nameof(GetAvailableVehicles), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de um veículo existente
    /// </summary>
    /// <param name="id">ID do veículo a ser atualizado</param>
    /// <param name="dto">Novos dados do veículo</param>
    /// <returns>Veículo atualizado</returns>
    /// <response code="200">Veículo atualizado com sucesso</response>
    /// <response code="400">ID não corresponde ou dados inválidos</response>
    /// <response code="404">Veículo não encontrado</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(VehicleDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var result = await useCaseController.UpdateVehicle(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista todos os veículos disponíveis para venda
    /// </summary>
    /// <returns>Lista de veículos disponíveis ordenados por preço</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
    public async Task<IActionResult> GetAvailableVehicles()
    {
        var result = await useCaseController.GetAvailableVehicles();
        return Ok(result);
    }

    /// <summary>
    /// Lista todos os veículos já vendidos
    /// </summary>
    /// <returns>Lista de veículos vendidos ordenados por preço</returns>
    /// <response code="200">Lista retornada com sucesso</response>
    [HttpGet("sold")]
    [ProducesResponseType(typeof(IEnumerable<VehicleSaleDto>), 200)]
    public async Task<IActionResult> GetSoldVehicles()
    {
        var result = await useCaseController.GetSoldVehicles();
        return Ok(result);
    }
    
    /// <summary>
    /// Remove um veículo do catálogo
    /// </summary>
    /// <param name="id">ID do veículo a ser removido</param>
    /// <returns>Status da operação de remoção</returns>
    /// <response code="200">Veículo removido com sucesso</response>
    /// <response code="400">ID inválido fornecido</response>
    /// <response code="404">Veículo não encontrado</response>
    /// <response code="409">Não é possível remover veículo vendido</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteVehicle(Guid id)
    {
        try
        {
            var success = await useCaseController.DeleteVehicle(id);
        
            if (success)
                return Ok(new { message = "Veículo removido com sucesso!" });
        
            return NotFound(new { message = "Veículo não encontrado" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Webhook para atualização do status de pagamento de vendas
    /// </summary>
    /// <param name="dto">Dados do webhook contendo código de pagamento e status</param>
    /// <returns>Confirmação da atualização do status</returns>
    /// <response code="200">Status de pagamento atualizado com sucesso</response>
    /// <response code="400">Dados do webhook inválidos</response>
    /// <response code="404">Código de pagamento não encontrado</response>
    [HttpPost("payment-webhook")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookDto dto)
    {
        var success = await useCaseController.UpdatePaymentStatus(dto);
        
        if (success)
            return Ok(new { message = "Status de pagamento atualizado com sucesso!" });
        
        return NotFound(new { message = "Código de pagamento não encontrado" });
    }
}