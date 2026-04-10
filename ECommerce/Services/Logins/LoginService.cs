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

        public async Task<string> FazerLogin(LoginEntrarDTO credenciais)
        {
            var login = await ProcurarLoginPorEmail(credenciais.Email);

            if (!BCrypt.Net.BCrypt.Verify(credenciais.Senha, login.Senha))
                throw new LoginCredenciaisInvalidasException();

            var token = "Login feito com sucesso.";
            return token;
        }

        private async Task<Login> ProcurarLoginPorEmail(string email)
        {
            var login = await _repository.ObterPorEmail(email);
            if (login == null)
                throw new LoginCredenciaisInvalidasException();
            return login;
        }

        private Login CriarLoginPorDTO(LoginCreateDTO novoLogin)
        {
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(novoLogin.Senha);
            return new Login(novoLogin.Email, senhaHash, novoLogin.IdUsuario);
        }

        public async Task<Login> CriarLogin(LoginCreateDTO novoLogin)
        {
            var login = CriarLoginPorDTO(novoLogin);
            await _repository.CriarLogin(login);
            return login;
        }

        public async Task DeletarLogin(string email)
        {
            var login = await ProcurarLoginPorEmail(email);
            await _repository.RemoverLogin(login);
        }
    }
}
