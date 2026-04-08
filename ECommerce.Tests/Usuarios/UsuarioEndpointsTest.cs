using ECommerce.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Usuarios
{
    public class UsuarioEndpointsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _url = "api/Usuarios";
        private readonly HttpClient _client;
        

        public UsuarioEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioInexistente_Retornar404()
        {
            var id = Guid.NewGuid().ToString();
            var response = await _client.GetAsync($"{_url}/{id}");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private UsuarioCreateDTO CriarNovoUsuarioCreateDTO()
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
                NumeroCasa = "400"
    };
        }

        [Fact]
        public async Task Deve_CriarNovoUsuario_Retorno201RespostaValida()
        {
            var usuario = CriarNovoUsuarioCreateDTO();

            var postResponse = await _client.PostAsJsonAsync(_url, usuario);
            postResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var usuarioCriado = await postResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            Assert.NotNull(usuarioCriado);
            Assert.Equal(usuario.Nome, usuarioCriado.Nome);
            Assert.Equal(usuario.Telefone, usuarioCriado.Telefone);
            Assert.Equal(usuario.Cep, usuarioCriado.Cep);
            Assert.Equal(usuario.Cidade, usuarioCriado.Cidade);
            Assert.Equal(usuario.Rua, usuarioCriado.Rua);
            Assert.Equal(usuario.NumeroCasa, usuarioCriado.NumeroCasa);
        }
    }   
}
