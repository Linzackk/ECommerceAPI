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
                NumeroCasa = "400"
            };
        }
        private UsuarioCreateDTO CriarUsuarioInvalido()
        {
            return new UsuarioCreateDTO()
            {
                Nome = "Nome Teste",
                Email = "email@email.com",
                Cpf = "12121212121",
                Telefone = "116666",
                Cep = "777",
                Cidade = "SP",
                Rua = "Rua do Teste",
                NumeroCasa = "400"
            };
        }

        [Fact]
        public async Task Deve_CriarNovoUsuario_Retorno201RespostaValida()
        {
            var usuario = CriarUsuarioValido();

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

        [Fact]
        public async Task Deve_CriarUsuarioInvalido_Retorno400()
        {
            var usuario = CriarUsuarioInvalido();

            var postResponse = await _client.PostAsJsonAsync(_url, usuario);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioInexistente_Retornar404()
        {
            var id = Guid.NewGuid().ToString();
            var response = await _client.GetAsync($"{_url}/{id}");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioExistente_Retorno200()
        {
            var usuario = CriarUsuarioValido();

            var response = await _client.PostAsJsonAsync(_url, usuario);
            response.EnsureSuccessStatusCode();

            var usuarioCriado = await response.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);

            var responseProcura = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            Assert.Equal(HttpStatusCode.OK, responseProcura.StatusCode);

            var usuarioProcura = await responseProcura.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioProcura);

            Assert.Equal(usuarioCriado.Id, usuarioProcura.Id);
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistenteComInformacoesValidas_Retorno204()
        {
            var usuario = CriarUsuarioValido();

            var response = await _client.PostAsJsonAsync(_url, usuario);
            response.EnsureSuccessStatusCode();

            var usuarioCriado = await response.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);

            var infoUpdate = new UsuarioUpdateDTO()
            {
                Nome = "Novo Nome Teste",
                NumeroCasa = "300"
            };

            var responseUpdate = await _client.PatchAsJsonAsync($"{_url}/{usuarioCriado.Id}", infoUpdate);
            responseUpdate.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistente_VerificarInformacoesAtualizadas()
        {
            var usuario = CriarUsuarioValido();

            var response = await _client.PostAsJsonAsync(_url, usuario);
            response.EnsureSuccessStatusCode();

            var usuarioCriado = await response.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);

            var infoUpdate = new UsuarioUpdateDTO()
            {
                Nome = "Novo Nome Teste",
                NumeroCasa = "300"
            };

            var responseUpdate = await _client.PatchAsJsonAsync($"{_url}/{usuarioCriado.Id}", infoUpdate);
            responseUpdate.EnsureSuccessStatusCode();

            var responseProcura = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            response.EnsureSuccessStatusCode();

            var usuarioAtualizado = await responseProcura.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);
            Assert.NotNull(usuarioAtualizado);
            Assert.Equal(infoUpdate.Nome, usuarioAtualizado.Nome);
            Assert.Equal(infoUpdate.NumeroCasa, usuarioAtualizado.NumeroCasa);
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistenteComInformacoesInvalidas_Retorno400()
        {
            var usuario = CriarUsuarioValido();

            var response = await _client.PostAsJsonAsync(_url, usuario);
            response.EnsureSuccessStatusCode();

            var usuarioCriado = await response.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);

            var infoUpdate = new UsuarioUpdateDTO()
            {
                Telefone = "Telefone2000",
                NumeroCasa = "300"
            };

            var responseUpdate = await _client.PatchAsJsonAsync($"{_url}/{usuarioCriado.Id}", infoUpdate);
            Assert.NotNull(responseUpdate);
            Assert.Equal(HttpStatusCode.BadRequest, responseUpdate.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarUsuarioExistente_Retorno204()
        {
            var usuario = CriarUsuarioValido();

            var response = await _client.PostAsJsonAsync(_url, usuario);
            response.EnsureSuccessStatusCode();

            var usuarioCriado = await response.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{usuarioCriado.Id}");
            Assert.NotNull(deleteResponse);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarUsuarioInexistente_Retorno204()
        {
            var id = Guid.NewGuid().ToString();
            var response = await _client.DeleteAsync($"{_url}/{id}");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }   
}
