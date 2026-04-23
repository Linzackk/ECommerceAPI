using ECommerce.DTOs.Login;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Logins;
using ECommerce.Services.Logins;
using ECommerce.Services.Tokens;
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
        static string senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaTeste);
        static string emailTeste = "email@email.com";
        static Guid usuarioIdTeste = Guid.NewGuid();

        private static Login CriarNovoLoginValido()
        {
            return new Login(emailTeste, senhaHash, usuarioIdTeste);
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
            var mockTokenService = new Mock<ITokenService>();

            mockTokenService.Setup(x => x.GerarToken(login))
                .Returns("Token");

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(login);

            var service = new LoginService(mock.Object, mockTokenService.Object);

            var resultado = await service.ProcurarLoginPorEmail(emailTeste);

            Assert.NotNull(resultado);
        }

        [Fact]
        public async Task CriarLogin_DeveLancarExcessao_DadosInvalidosParaCriar()
        {
            var login = CriarNovoLoginDTOInvalido();

            var mock = new Mock<ILoginRepository>();
            var tokenService = new Mock<ITokenService>();

            var service = new LoginService(mock.Object, tokenService.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.CriarLogin(login));
        }

        [Fact]
        public async Task CriarLogin_DeveCriarLogin_BuscarEValidarInformacoes()
        {
            var loginResponse = CriarNovoLoginValido();
            var loginCreate = CriarNovoLoginDTOValido();
            var login = CriarNovoLoginValido();

            var mock = new Mock<ILoginRepository>();

            var mockTokenService = new Mock<ITokenService>();

            mockTokenService.Setup(x => x.GerarToken(login))
                .Returns("Token");

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(login);

            var service = new LoginService(mock.Object, mockTokenService.Object);

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
            var loginResponse = CriarNovoLoginValido();
            var loginCreate = CriarNovoLoginDTOValido();
            var login = CriarNovoLoginValido();

            var mock = new Mock<ILoginRepository>();

            var mockTokenService = new Mock<ITokenService>();

            var service = new LoginService(mock.Object, mockTokenService.Object);

            await Assert.ThrowsAsync<LoginCredenciaisInvalidasException>(() => service.ProcurarLoginPorEmail(emailTeste));
        }

        [Fact]
        public async Task FazerLogin_DeveCriarLogin_RetornarTokenValido()
        {
            var loginResponse = CriarNovoLoginValido();
            var loginCreate = CriarNovoLoginDTOValido();
            var loginEntrar = new LoginEntrarDTO();

            loginEntrar.Email = emailTeste;
            loginEntrar.Senha = senhaTeste;

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(loginResponse);

            var mockTokenService = new Mock<ITokenService>();

            mockTokenService.Setup(x => x.GerarToken(loginResponse))
                .Returns("Token");

            var service = new LoginService(mock.Object, mockTokenService.Object);

            await service.CriarLogin(loginCreate);

            var response = await service.FazerLogin(loginEntrar);

            Assert.NotNull(response);
            Assert.Equal("Token", response);
        }
    }
}
