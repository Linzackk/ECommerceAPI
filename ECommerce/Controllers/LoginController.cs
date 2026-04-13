using ECommerce.DTOs.Login;
using ECommerce.Models;
using ECommerce.Services.Logins;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _service;
        public LoginController(ILoginService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> FazerLogin([FromBody] LoginEntrarDTO credenciais)
        {
            var token = await _service.FazerLogin(credenciais);
            return Ok(token);
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            return Ok(await _service.ObterTodos());
        }
    }
}
