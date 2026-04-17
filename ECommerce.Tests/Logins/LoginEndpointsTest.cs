using ECommerce.DTOs.Login;
using ECommerce.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Logins
{
    public class LoginEndpointsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _urlLogin = "api/Login";
        private readonly string _urlUsuario = "api/Usuarios";
        private readonly HttpClient _client;
        private CreateUsuarioHelper helper;

        public LoginEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            helper = new CreateUsuarioHelper(_client);
        }
        private LoginEntrarDTO CriarLoginEntrarValido(string email, string senha)
        {
            return new LoginEntrarDTO()
            {
                Email = email,
                Senha = senha
            };
        }

        private LoginEntrarDTO CriarLoginEntrarInvalido()
        {
            return new LoginEntrarDTO()
            {
                Email = "qualquerCoisaAi@Invalido.com",
                Senha = "Senha Incorreta"
            };
        }

        [Fact]
        public async Task Deve_CriarNovoLogin_EntrarComCredenciais_Retorno200()
        {
            var usuario = await helper.CriarUsuarioValido_NoContexto();
            var usuarioInfos = helper.CriarUsuarioValido();

            var loginEntrar = CriarLoginEntrarValido(usuarioInfos.Email, usuarioInfos.Senha);

            var loginPostResponse = await _client.PostAsJsonAsync(_urlLogin, loginEntrar);
            loginPostResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, loginPostResponse.StatusCode);

            var response = await loginPostResponse.Content.ReadAsStringAsync();

            Assert.NotNull(response);
            Assert.Equal("Login feito com sucesso.", response);
        }

        [Fact]
        public async Task Deve_CriarNovoLogin_EntrarComCredenciaisInvalidas_Retorno400()
        {
            var usuario = await helper.CriarUsuarioValido_NoContexto();
            var loginEntrar = CriarLoginEntrarInvalido();

            var loginPostResponse = await _client.PostAsJsonAsync(_urlLogin, loginEntrar);

            Assert.Equal(HttpStatusCode.BadRequest, loginPostResponse.StatusCode);

            var response = await loginPostResponse.Content.ReadAsStringAsync();

            Assert.NotNull(response);
        }
    }
}
