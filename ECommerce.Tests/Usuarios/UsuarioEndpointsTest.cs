using Azure.Core;
using ECommerce.DTOs.Usuarios;
using ECommerce.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Usuarios
{
    public class UsuarioEndpointsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _url = "api/Usuarios";
        private readonly HttpClient _client;
        private LoginHelper loginHelper;
        private UsuarioHelper helper;
        private readonly Guid IdTeste = Guid.NewGuid();
        private readonly CustomWebApplicationFactory _factory;

        public UsuarioEndpointsTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            helper = new UsuarioHelper(_client);
            loginHelper = new LoginHelper(_client);
        }

        [Fact]
        public async Task Deve_CriarNovoUsuario_Retorno201RespostaValida()
        {
            var usuario = helper.CriarUsuarioValido();

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
            var usuario = helper.CriarUsuarioInvalido();

            var postResponse = await _client.PostAsJsonAsync(_url, usuario);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioExistente_Retorno200()
        {
            var usuario = helper.CriarUsuarioValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);
            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var responseProcura = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            Assert.Equal(HttpStatusCode.OK, responseProcura.StatusCode);

            var usuarioProcura = await responseProcura.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioProcura);
            Assert.Equal(usuarioCriado.Id, usuarioProcura.Id);
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioInexistente_Retornar404()
        {
            var usuario = helper.CriarAdminValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var response = await _client.GetAsync($"{_url}/{IdTeste}");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistenteComInformacoesValidas_Retorno204()
        {
            var usuario = helper.CriarUsuarioValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var infoUpdate = helper.CriarAtualizacaoValida();

            var responseUpdate = await _client.PatchAsJsonAsync($"{_url}/{usuarioCriado.Id}", infoUpdate);
            responseUpdate.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistente_VerificarInformacoesAtualizadas()
        {
            var usuario = helper.CriarUsuarioValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var infoUpdate = helper.CriarAtualizacaoValida();

            var responseUpdate = await _client.PatchAsJsonAsync($"{_url}/{usuarioCriado.Id}", infoUpdate);
            responseUpdate.EnsureSuccessStatusCode();

            var responseProcura = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            responseProcura.EnsureSuccessStatusCode();

            var usuarioAtualizado = await responseProcura.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            Assert.NotNull(usuarioCriado);
            Assert.NotNull(usuarioAtualizado);
            Assert.Equal(infoUpdate.Nome, usuarioAtualizado.Nome);
            Assert.Equal(infoUpdate.NumeroCasa, usuarioAtualizado.NumeroCasa);
        }

        [Fact]
        public async Task Deve_AtualizarUsuarioExistenteComInformacoesInvalidas_Retorno400()
        {
            var usuario = helper.CriarUsuarioValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

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
            var usuario = helper.CriarUsuarioValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{usuarioCriado.Id}");
            Assert.NotNull(deleteResponse);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarUsuarioInexistente_Retorno404()
        {
            var usuario = helper.CriarAdminValido();
            var usuarioCriado = await helper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var response = await _client.DeleteAsync($"{_url}/{IdTeste}");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }   
}
