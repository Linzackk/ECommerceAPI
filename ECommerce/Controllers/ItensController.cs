using ECommerce.DTOs.Itens;
using ECommerce.Services.Itens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ItensController : ControllerBase
    {
        private readonly IItemService _service;
        public ItensController(IItemService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> CriarNovoItem([FromBody] ItemCreateDTO novoItem)
        {
            var itemCriado = await _service.CriarNovoItem(novoItem);
            return CreatedAtAction(nameof(ObterPorId), new { Id = itemCriado.Id }, itemCriado);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var item = await _service.ObterItemPorId(id);
            return Ok(item);
        }

        [HttpPatch("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> AtualizarItem([FromBody] ItemUpdateDTO itemAtualizado, Guid id)
        {
            await _service.AtualizarItem(itemAtualizado, id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> RemoverItem(Guid id)
        {
            await _service.RemoverItem(id);
            return NoContent();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ObterTodos()
        {
            var itens = await _service.ObterTodos();
            return Ok(itens);
        }
    }
}
