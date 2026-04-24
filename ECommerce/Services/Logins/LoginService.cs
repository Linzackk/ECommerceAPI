using ECommerce.DTOs.Login;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Logins;
using ECommerce.Services.Tokens;

namespace ECommerce.Services.Logins
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;
        private readonly ITokenService _tokenService;
        public LoginService(ILoginRepository repository, ITokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyCollection<Login>> ObterTodos()
        {
            return await _repository.ObterTodos();
        }

        public async Task<string> FazerLogin(LoginEntrarDTO credenciais)
        {
            var login = await ProcurarLoginPorEmail(credenciais.Email);

            if (!BCrypt.Net.BCrypt.Verify(credenciais.Senha, login.Senha))
                throw new LoginCredenciaisInvalidasException();

            var token = _tokenService.GerarToken(login);
            return token;
        }

        public async Task<Login> ProcurarLoginPorEmail(string email)
        {
            var login = await _repository.ObterPorEmail(email);
            if (login == null)
                throw new LoginCredenciaisInvalidasException();
            return login;
        }

        private Login CriarLoginPorDTO(LoginCreateDTO novoLogin)
        {        
            return new Login(novoLogin.Email, novoLogin.Senha, novoLogin.IdUsuario);
        }

        public async Task CriarLogin(LoginCreateDTO novoLogin)
        {
            var login = CriarLoginPorDTO(novoLogin);

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(novoLogin.Senha);
            login.DefinirNovaSenha(senhaHash);

            await _repository.CriarLogin(login);
        }

        public async Task DeletarLogin(Guid usuarioId)
        {
            var login = await ProcurarLoginPorUsuarioId(usuarioId);
            if (login == null)
                throw new UsuarioNotFound();
            await _repository.RemoverLogin(login);
        }

        public async Task<Login?> ProcurarLoginPorUsuarioId(Guid usuarioId)
        {
            var resultado = await _repository.ProcurarLoginPorUsuarioId(usuarioId);
            return resultado;
        }
    }
}
