using ECommerce.DTOs.Usuarios;
using ECommerce.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _service;
        public UsuariosController(IUsuariosService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioCreateDTO novoUsuario)
        {
            var usuarioCriado = await _service.CriarNovoUsuario(novoUsuario);
            return CreatedAtAction(nameof(ObterUsuarioPorId), new { Id = usuarioCriado.Id }, usuarioCriado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterUsuarioPorId(Guid id)
        {
            var usuario = await _service.ObterUsuarioPorId(id);
            return Ok(usuario);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> AtualizarUsuario([FromBody] UsuarioUpdateDTO usuarioAtualizado, Guid id)
        {
            await _service.AtualizarUsuario(id, usuarioAtualizado);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarUsuario(Guid id)
        {
            await _service.RemoverUsuario(id);
            return NoContent();
        }
    }
}
