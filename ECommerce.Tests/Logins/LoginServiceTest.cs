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

        private static Login CriarNovoLogin()
        {
            return new Login(emailTeste, senhaTeste, Guid.NewGuid());
        }
        [Fact]
        public async Task CriarLogin_DeveCriarLogin_BuscarLoginCriado()
        {
            var login = CriarNovoLogin();

            var mock = new Mock<ILoginRepository>();

            mock.Setup(x => x.ObterPorEmail(emailTeste))
                .ReturnsAsync(login);

            var service = new LoginService(mock.Object);

            var resultado = await service.ProcurarLoginPorEmail(emailTeste);

            Assert.NotNull(resultado);
            Assert.Equal(login.Email, resultado.Email);
            Assert.Equal(login.IdUsuario, resultado.IdUsuario);
        }
    }
}
