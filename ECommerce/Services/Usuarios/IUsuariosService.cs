using ECommerce.Models;

namespace ECommerce.Services.Usuarios
{
    public interface IUsuariosService
    {
        Task<UsuarioResponseDTO> CriarNovoUsuario(UsuarioCreateDTO novoUsuario);
        Task<Usuario> ObterUsuarioPorId(Guid usuarioId);

        Task RemoverUsuario(Guid usuarioId);
        Task AtualizarUsuario(Guid usuarioId);
    }
}
