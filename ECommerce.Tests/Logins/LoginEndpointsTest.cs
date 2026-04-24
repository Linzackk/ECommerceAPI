using ECommerce.DTOs.Login;
using ECommerce.DTOs.Usuarios;
using ECommerce.Models;
using ECommerce.Tests.Helpers;
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
        private readonly UsuarioHelper usuarioHelper;
        private readonly LoginHelper loginHelper;

        public LoginEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            usuarioHelper = new UsuarioHelper(_client);
            loginHelper = new LoginHelper(_client);
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
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);
            var usuarioInfos = usuarioHelper.CriarUsuarioValido();

            var loginEntrar = CriarLoginEntrarValido(usuarioInfos.Email, usuarioInfos.Senha);

            var loginPostResponse = await _client.PostAsJsonAsync(_urlLogin, loginEntrar);
            loginPostResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, loginPostResponse.StatusCode);

            var response = await loginPostResponse.Content.ReadAsStringAsync();

            Assert.NotNull(response);
            Assert.NotEqual(string.Empty, response);
        }

        [Fact]
        public async Task Deve_CriarNovoLogin_EntrarComCredenciaisInvalidas_Retorno400()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);
            var loginEntrar = CriarLoginEntrarInvalido();

            var loginPostResponse = await _client.PostAsJsonAsync(_urlLogin, loginEntrar);

            Assert.Equal(HttpStatusCode.BadRequest, loginPostResponse.StatusCode);

            var response = await loginPostResponse.Content.ReadAsStringAsync();

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Deve_AcessarEndpointSemAutenticacao_Retorno401()
        {
            var getResponse = await _client.GetAsync(_urlLogin);
            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AcessarEnpointAdminOnlyUsuarioComum_Retorno403()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var getResponse = await _client.GetAsync(_urlLogin);
            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }
    }
}
