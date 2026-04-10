using ECommerce.DTOs.Login;

namespace ECommerce.Services.Logins
{
    public interface ILoginService
    {
        Task<string> FazerLogin(LoginEntrarDTO credenciais);
        Task CriarLogin(LoginCreateDTO novoLogin);
        Task DeletarLogin(string email);
    }
}
