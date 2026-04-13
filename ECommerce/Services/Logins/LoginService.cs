using ECommerce.DTOs.Login;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Logins;

namespace ECommerce.Services.Logins
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;

        public LoginService(ILoginRepository repository)
        {
            _repository = repository;
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

            var token = "Login feito com sucesso.";
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
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(novoLogin.Senha);
            Console.WriteLine($"NOVO LOGIN CRIADO COM INFOS: {novoLogin.Email}, {novoLogin.Senha}, {novoLogin.IdUsuario}");
            return new Login(novoLogin.Email, senhaHash, novoLogin.IdUsuario);
        }

        public async Task CriarLogin(LoginCreateDTO novoLogin)
        {
            var login = CriarLoginPorDTO(novoLogin);
            Console.WriteLine($"NOVO LOGIN CRIADO COM INFOS: {login.Email}, {login.Senha}, {login.IdUsuario}");
            await _repository.CriarLogin(login);
        }

        public async Task DeletarLogin(Guid usuarioId)
        {
            var login = await ProcurarLoginPorUsuarioId(usuarioId);
            await _repository.RemoverLogin(login);
        }

        public async Task<Login> ProcurarLoginPorUsuarioId(Guid usuarioId)
        {
            return await _repository.ProcurarLoginPorUsuarioId(usuarioId);
        }
    }
}
