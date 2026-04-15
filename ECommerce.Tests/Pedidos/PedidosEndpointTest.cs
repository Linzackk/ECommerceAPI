using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Pedidos
{
    public class PedidosEndpointTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _url = "api/Pedidos";
        private readonly string _urlUsuario = "api/Usuarios";
        private readonly Guid IdTeste = Guid.NewGuid();

        public PedidosEndpointTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private PedidoCreateDTO CriarPedidoValido(Guid idUsuario)
        {
            return new PedidoCreateDTO() { IdUsuario = idUsuario };
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

        [Fact]
        public async Task Deve_CriarPedidoVazio_Retorno201()
        {
            var usuario = CriarUsuarioValido();

            var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            userPostResponse.EnsureSuccessStatusCode();
            var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            var pedido = CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_LancarErroNotFound_CriarPedidoSemUsuario()
        {
            var pedido = CriarPedidoValido(Guid.Empty);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);

            Assert.Equal(HttpStatusCode.NotFound, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_LancarErroBadRequest_CriarPedidoComPedidoJaAberto()
        {
            var usuario = CriarUsuarioValido();

            var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            userPostResponse.EnsureSuccessStatusCode();
            var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            var pedido = CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();
            
            var postBadRequest = await _client.PostAsJsonAsync(_url, pedido);

            Assert.Equal(HttpStatusCode.BadRequest, postBadRequest.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarPedido_Retorno204()
        {
            var usuario = CriarUsuarioValido();

            var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            userPostResponse.EnsureSuccessStatusCode();
            var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            var pedido = CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();

            var pedidoCriado = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();

            var deleteResponse = await _client.DeleteAsync($"{_url}/{pedidoCriado.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarPedidoInexistente_Retorno404()
        {
            var usuario = CriarUsuarioValido();

            var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            userPostResponse.EnsureSuccessStatusCode();
            var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            var deleteResponse = await _client.DeleteAsync($"{_url}/{IdTeste}");

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }
        
    }
}
