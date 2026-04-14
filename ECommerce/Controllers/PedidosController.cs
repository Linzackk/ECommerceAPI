using ECommerce.DTOs.Pedidos;
using ECommerce.Services.Pedidos;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;
        public PedidosController(IPedidoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoCreateDTO novoPedido)
        {
            var pedido = await _service.CriarNovoPedido(novoPedido);
            return CreatedAtAction(nameof(ObterPedidoPorId), new { Id = pedido.Id }, pedido);
        }

        [HttpGet("{usuarioId}")] // Com JWT vou remover e manter só o get e então pegar todos do id do usuario contido no JWT
        public async Task<IActionResult> ObterPedidoPorUsuarioId(Guid usuarioId)
        {
            var pedidos = await _service.ObterTodosPedidosUsuario(usuarioId);
            return Ok(pedidos);
        }

        [HttpPatch("{pedidoId}")]
        public async Task<IActionResult> FinalizarPedido(Guid pedidoId)
        {
            await _service.FinalizarPedido(pedidoId);
            return NoContent();
        }

        [HttpDelete("{pedidoId}")]
        public async Task<IActionResult> RemoverPedido(Guid pedidoId)
        {
            await _service.RemoverPedido(pedidoId);
            return NoContent();
        }

        [HttpPost("{pedidoId}/Itens")]
        public async Task<IActionResult> AdicionarItemAoPedido([FromBody] PedidoItemCreateDTO novoPedidoItem)
        {
            await _service.AdicionarItemNoPedido(novoPedidoItem);
            return Ok();
        }

        [HttpGet("{pedidoId}/Itens")]
        public async Task<IActionResult> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await _service.ObterPedidoPorId(pedidoId);
            return Ok(pedido);
        }

        [HttpPatch("{pedidoId}/Itens")]
        public async Task<IActionResult> AtualizarItemNoPedido([FromBody] PedidoItemUpdateDTO pedidoItemAtualizado)
        {
            await _service.AtualizarItemNoPedido(pedidoItemAtualizado);
            return NoContent();
        }

        [HttpDelete("{pedidoId}/Itens")]
        public async Task<IActionResult> RemoverItemDoPedido([FromBody] PedidoItemRemoveDTO pedidoItemRemove)
        {
            await _service.RemoverItemNoPedido(pedidoItemRemove);
            return NoContent();
        }
    }
}
