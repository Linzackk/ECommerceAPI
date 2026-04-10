using ECommerce.DTOs.Login;
using ECommerce.Models;

namespace ECommerce.Services.Logins
{
    public interface ILoginService
    {
        Task<string> FazerLogin(LoginEntrarDTO credenciais);
        Task CriarLogin(LoginCreateDTO novoLogin);
        Task DeletarLogin(Guid usuarioId);
        Task<IReadOnlyCollection<Login>> ObterTodos();
    }
}
