using ECommerce.DTOs.Login;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Logins;
using ECommerce.Services.Logins;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Logins
{
    public class LoginServiceTest
    {
        static string senhaTeste = "senhaSenha";
        static string emailTeste = "email@email.com";

        private static Login CriarNovoLoginValido()
        {
            return new Login(emailTeste, senhaTeste, Guid.NewGuid());
        }

        private static LoginCreateDTO CriarNovoLoginDTOValido()
        {
            var login = new LoginCreateDTO();
            login.Email = emailTeste;
            login.Senha = senhaTeste;
            login.IdUsuario = Guid.NewGuid();
            return login;
        }

        private static LoginCreateDTO CriarNovoLoginDTOInvalido()
        {
            return new LoginCreateDTO();
        }

        [Fact]
        public async Task CriarLogin_DeveCriarLogin_BuscarLoginCriado()
        {
            var login = CriarNovoLoginValido();

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(login);

            var service = new LoginService(mock.Object);

            var resultado = await service.ProcurarLoginPorEmail(emailTeste);

            Assert.NotNull(resultado);
            Assert.Equal(login.Email, resultado.Email);
            Assert.Equal(login.IdUsuario, resultado.IdUsuario);
        }

        [Fact]
        public async Task CriarLogin_DeveLancarExcessao_DadosInvalidosParaCriar()
        {
            var login = CriarNovoLoginDTOInvalido();

            var mock = new Mock<ILoginRepository>();

            var service = new LoginService(mock.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.CriarLogin(login));
        }
    }
}
