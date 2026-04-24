using ECommerce.DTOs.Pedidos;
using ECommerce.Services.Pedidos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;
        public PedidosController(IPedidoService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoCreateDTO novoPedido)
        {
            var userIdClaim = User.FindFirst("sub")?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || Guid.Parse(userIdClaim) != novoPedido.IdUsuario)
                return Forbid();
            var pedido = await _service.CriarNovoPedido(novoPedido);

            return Ok(pedido);
            //return CreatedAtAction(nameof(ObterPedidoPorId), new { Id = pedido.Id }, pedido);
        }

        [HttpGet("{usuarioId}")] // Com JWT vou remover e manter só o get e então pegar todos do id do usuario contido no JWT
        [Authorize]
        public async Task<IActionResult> ObterPedidoPorUsuarioId(Guid usuarioId)
        {
            var pedidos = await _service.ObterTodosPedidosUsuario(usuarioId);
            return Ok(pedidos);
        }

        [HttpPatch("{pedidoId}")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> FinalizarPedido(Guid pedidoId)
        {
            await _service.FinalizarPedido(pedidoId);
            return NoContent();
        }

        [HttpDelete("{pedidoId}")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> RemoverPedido(Guid pedidoId)
        {
            await _service.RemoverPedido(pedidoId);
            return NoContent();
        }

        [HttpPost("{pedidoId}/Itens")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> AdicionarItemAoPedido([FromBody] PedidoItemCreateDTO novoPedidoItem, Guid pedidoId)
        {
            await _service.AdicionarItemNoPedido(novoPedidoItem, pedidoId);
            return Ok();
        }

        [HttpGet("{pedidoId}/Itens")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await _service.ObterPedidoPorId(pedidoId);
            return Ok(pedido);
        }

        [HttpPatch("{pedidoId}/Itens")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> AtualizarItemNoPedido([FromBody] PedidoItemUpdateDTO pedidoItemAtualizado, Guid pedidoId)
        {
            await _service.AtualizarItemNoPedido(pedidoItemAtualizado, pedidoId);
            return NoContent();
        }

        [HttpDelete("{pedidoId}/Itens/{pedidoItemRemove}")]
        [Authorize(Policy = "IsOrderOwnerOrAdmin")]
        public async Task<IActionResult> RemoverItemDoPedido(Guid pedidoItemRemove, Guid pedidoId)
        {
            await _service.RemoverItemNoPedido(pedidoItemRemove, pedidoId);
            return NoContent();
        }
    }
}
