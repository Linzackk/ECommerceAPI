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
        static Guid usuarioIdTeste = Guid.NewGuid();

        private static Login CriarNovoLoginValido()
        {
            return new Login(emailTeste, senhaTeste, usuarioIdTeste);
        }

        private static LoginCreateDTO CriarNovoLoginDTOValido()
        {
            var login = new LoginCreateDTO();
            login.Email = emailTeste;
            login.Senha = senhaTeste;
            login.IdUsuario = usuarioIdTeste;
            return login;
        }

        private static LoginCreateDTO CriarNovoLoginDTOInvalido()
        {
            return new LoginCreateDTO();
        }

        [Fact]
        public async Task BuscaValida_DeveBuscarLogin_LoginNaoNulo()
        {
            var login = CriarNovoLoginValido();

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(login);

            var service = new LoginService(mock.Object);

            var resultado = await service.ProcurarLoginPorEmail(emailTeste);

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task CriarLogin_DeveLancarExcessao_DadosInvalidosParaCriar()
        {
            var login = CriarNovoLoginDTOInvalido();

            var mock = new Mock<ILoginRepository>();

            var service = new LoginService(mock.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.CriarLogin(login));
        }

        [Fact]
        public async Task CriarLogin_DeveCriarLogin_BuscarEValidarInformacoes()
        {
            var loginResponse = CriarNovoLoginValido();
            var loginCreate = CriarNovoLoginDTOValido();

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(loginResponse);

            var service = new LoginService(mock.Object);

            await service.CriarLogin(loginCreate);

            var response = await service.ProcurarLoginPorEmail(emailTeste);

            Assert.NotNull(response);
            Assert.Equal(loginCreate.Email, response.Email);
            Assert.Equal(loginCreate.IdUsuario, response.IdUsuario);

            mock.Verify(x => x.CriarLogin(It.IsAny<Login>()), Times.Once);
        }

        [Fact]
        public async Task ProcurarLogin_DeveLancarErro_QuandoLoginNaoExistir()
        {
            var login = CriarNovoLoginValido();

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync((Login?)null);

            var service = new LoginService(mock.Object);

            await Assert.ThrowsAsync<LoginCredenciaisInvalidasException>(() => service.ProcurarLoginPorEmail(emailTeste));
        }
    }
}
