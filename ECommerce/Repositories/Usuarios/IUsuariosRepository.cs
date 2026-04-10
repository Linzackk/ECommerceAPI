using ECommerce.Models;

namespace ECommerce.Repositories.Usuarios
{
    public interface IUsuariosRepository
    {
        Task CriarUsuario(Usuario novoUsuario);
        Task<Usuario?> ObterUsuarioPorId(Guid usuarioId);
        Task AtualizarUsuario(Usuario usuarioAtualizado);
        Task RemoverUsuario(Usuario usuario);
        Task<IReadOnlyCollection<Usuario>> ObterTodos();
    }
}
