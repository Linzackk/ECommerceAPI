using ECommerce.DTOs.Itens;
using ECommerce.Services.Itens;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItensController : ControllerBase
    {
        private readonly IItemService _service;
        public ItensController(IItemService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarNovoItem([FromBody] ItemCreateDTO novoItem)
        {
            var itemCriado = await _service.CriarNovoItem(novoItem);
            return CreatedAtAction(nameof(ObterPorId), new { Id = itemCriado.Id }, itemCriado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var item = await _service.ObterItemPorId(id);
            return Ok(item);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> AtualizarItem([FromBody] ItemUpdateDTO itemAtualizado, Guid id)
        {
            await _service.AtualizarItem(itemAtualizado, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverItem(Guid id)
        {
            await _service.RemoverItem(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var itens = await _service.ObterTodos();
            return Ok(itens);
        }
    }
}
