using ECommerce.Models;

namespace ECommerce.Repositories.Logins
{
    public interface ILoginRepository
    {
        Task<Login?> ObterPorEmail(string email);
        Task CriarLogin(Login novoLogin);
        Task RemoverLogin(Login login);
    }
}
