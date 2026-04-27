using AutoMapper;
using ECommerce.DTOs.Login;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Logins;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Usuarios
{
    public class UsuariosService : IUsuariosService
    {
        private readonly IUsuariosRepository _repository;
        private readonly ILoginService _loginService;
        private readonly IMapper _mapper;
        public UsuariosService(IUsuariosRepository repository, ILoginService loginService, IMapper mapper)
        {
            _repository = repository;
            _loginService = loginService;
            _mapper = mapper;
        }

        public async Task<UsuarioResponseDTO> CriarNovoUsuario(UsuarioCreateDTO novoUsuario)
        {
            var usuario = _mapper.Map<Usuario>(novoUsuario);

            var novoLogin = new LoginCreateDTO() { Email = usuario.Email, Senha = novoUsuario.Senha, IdUsuario = usuario.Id };

            await _repository.CriarUsuario(usuario);
            await _loginService.CriarLogin(novoLogin);

            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        public async Task<UsuarioResponseDTO> ObterUsuarioPorId(Guid id)
        {
            Usuario usuario = await ObterUsuarioRepository(id);

            return _mapper.Map<UsuarioResponseDTO>(usuario);
        }

        private async Task<Usuario> ObterUsuarioRepository(Guid id)
        {
            var usuario = await _repository.ObterUsuarioPorId(id);
            if (usuario == null)
            {
                throw new UsuarioNotFound();
            }
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
            await _loginService.DeletarLogin(usuario.Id);
            await _repository.RemoverUsuario(usuario);
        }

        public async Task<IReadOnlyCollection<Usuario>> ObterTodos()
        {
            return await _repository.ObterTodos();
        }
    }
}
