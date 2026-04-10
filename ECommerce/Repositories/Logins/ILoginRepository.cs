using ECommerce.Models;

namespace ECommerce.Repositories.Logins
{
    public interface ILoginRepository
    {
        Task<Login?> ObterPorEmail(string email);
        Task<Login?> ProcurarLoginPorUsuarioId(Guid usuarioId);
        Task CriarLogin(Login novoLogin);
        Task RemoverLogin(Login login);

        Task<IReadOnlyCollection<Login>> ObterTodos();
    }
}
