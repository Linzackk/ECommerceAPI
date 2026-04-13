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

        public LoginEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private UsuarioCreateDTO CriarUsuarioValido()
        {
            return new UsuarioCreateDTO()
            {
                Nome = "Nome Teste",
                Email = "email@email.com",
                Cpf = "12121212121",
                Telefone = "11999999999",
                Cep = "00000000",
                Cidade = "SP",
                Rua = "Rua do Teste",
                NumeroCasa = "400",
                Senha = "senhaTeste"
            };
        }

        private LoginEntrarDTO CriarLoginEntrarValido()
        {
            return new LoginEntrarDTO()
            {
                Email = "email@email.com",
                Senha = "senhaTeste"
            };
        }

        private LoginEntrarDTO CriarLoginEntrarInvalido()
        {
            return new LoginEntrarDTO()
            {
                Email = "Email@Invalido.com",
                Senha = "Senha Incorreta"
            };
        }

        [Fact]
        public async Task Deve_CriarNovoLogin_EntrarComCredenciais_Retorno200()
        {
            var usuario = CriarUsuarioValido();
            var loginEntrar = CriarLoginEntrarValido();

            var postResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            postResponse.EnsureSuccessStatusCode();

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
            var usuario = CriarUsuarioValido();
            var loginEntrar = CriarLoginEntrarInvalido();

            var postResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            postResponse.EnsureSuccessStatusCode();

            var loginPostResponse = await _client.PostAsJsonAsync(_urlLogin, loginEntrar);

            Assert.Equal(HttpStatusCode.BadRequest, loginPostResponse.StatusCode);

            var response = await loginPostResponse.Content.ReadAsStringAsync();

            Assert.NotNull(response);
        }
    }
}
