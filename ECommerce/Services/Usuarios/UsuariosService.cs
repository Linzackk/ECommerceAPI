using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Usuarios;

namespace ECommerce.Services.Usuarios
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _repository;
        // adicionar service de login
        public UsuariosService(IUsuariosRepository repository)
        {
            _repository = repository;
        }

        public async Task<UsuarioResponseDTO> CriarNovoUsuario(UsuarioCreateDTO novoUsuario)
        {
            Usuario usuario = CriarModelPorDTO(novoUsuario);

            await _repository.CriarUsuario(usuario);

            return CriarResponseDTO(usuario);
        }

        private static Usuario CriarModelPorDTO(UsuarioCreateDTO novoUsuario)
        {
            return new Usuario(
                novoUsuario.Nome,
                novoUsuario.Telefone,
                novoUsuario.Rua,
                novoUsuario.Cidade,
                novoUsuario.NumeroCasa,
                novoUsuario.Cep,
                novoUsuario.Cpf
            );
        }

        private UsuarioResponseDTO CriarResponseDTO(Usuario usuario)
        {
            var response = new UsuarioResponseDTO();
            response.Id = usuario.Id;
            response.Nome = usuario.Nome;
            response.Telefone = usuario.Telefone;
            response.Rua = usuario.Rua;
            response.Cidade = usuario.Cidade;
            response.NumeroCasa = usuario.Numero;
            response.Cep = usuario.Cep;

            return response;
        }

        public async Task<UsuarioResponseDTO> ObterUsuarioPorId(Guid id)
        {
            Usuario usuario = await ObterUsuarioRepository(id);

            return CriarResponseDTO(usuario);
        }

        private async Task<Usuario> ObterUsuarioRepository(Guid id)
        {
            var usuario = await _repository.ObterUsuarioPorId(id);
            if (usuario == null)
                throw new UsuarioNotFound();
            return usuario;
        }

        public async Task AtualizarUsuario(Guid id, UsuarioUpdateDTO usuarioAtualizado)
        {
            if (
                usuarioAtualizado.Nome == null &&
                usuarioAtualizado.Telefone == null &&
                usuarioAtualizado.Rua == null &&
                usuarioAtualizado.Cidade == null &&
                usuarioAtualizado.NumeroCasa == null &&
                usuarioAtualizado.Cep == null
            )
                throw new ParametroInvalidoException("Nenhuma Informação foi fornecida para atualizar.");

            var usuario = await ObterUsuarioRepository(id);

            if (usuarioAtualizado.Nome != null)
                usuario.AtualizarNome(usuarioAtualizado.Nome);

            if (usuarioAtualizado.Telefone != null)
                usuario.AtualizarTelefone(usuarioAtualizado.Telefone);

            if (usuarioAtualizado.Rua != null)
                usuario.AtualizarRua(usuarioAtualizado.Rua);

            if (usuarioAtualizado.Cidade != null)
                usuario.AtualizarCidade(usuarioAtualizado.Cidade);

            if (usuarioAtualizado.NumeroCasa != null)
                usuario.AtualizarNumero(usuarioAtualizado.NumeroCasa);

            if (usuarioAtualizado.Cep != null)
                usuario.AtualizarCep(usuarioAtualizado.Cep);

            await _repository.AtualizarUsuario(usuario);
        }

        public async Task RemoverUsuario(Guid usuarioId)
        {
            var usuario = await ObterUsuarioRepository(usuarioId);
            await _repository.RemoverUsuario(usuario);
        }
    }
}
