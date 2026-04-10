using ECommerce.DTOs.Login;
using ECommerce.Models;

namespace ECommerce.Services.Logins
{
    public interface ILoginService
    {
        Task<string> FazerLogin(LoginEntrarDTO credenciais);
        Task<Login> CriarLogin(LoginCreateDTO novoLogin);
        Task DeletarLogin(string email);
    }
}
