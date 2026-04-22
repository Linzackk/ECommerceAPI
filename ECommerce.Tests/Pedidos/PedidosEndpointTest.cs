using ECommerce.DTOs.Itens;
using ECommerce.DTOs.Pedidos;
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

namespace ECommerce.Tests.Pedidos
{
    public class PedidosEndpointTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly string _url = "api/Pedidos";

        private readonly Guid IdTeste = Guid.NewGuid();
        private readonly PedidoHelper pedidoHelper;
        private readonly UsuarioHelper usuarioHelper;
        private readonly ItemHelper itemHelper;

        public PedidosEndpointTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            pedidoHelper = new PedidoHelper(_client);
            usuarioHelper = new UsuarioHelper(_client);
            itemHelper = new ItemHelper(_client);
        }
        private PedidoCreateDTO CriarPedidoValido(Guid idUsuario)
        {
            return new PedidoCreateDTO() { IdUsuario = idUsuario };
        }
        private async Task<PedidoResponseDTO> CriarUsuarioEPedidoNoContexto()
        {
            var usuarioCriado = await CriarUsuarioNoContexto();
            var pedidoCriado = await CriarPedidoNoContexto(usuarioCriado.Id);
            return pedidoCriado;
        }

        private async Task<PedidoResponseDTO> CriarPedidoNoContexto(Guid usuarioId)
        {
            var pedido = CriarPedidoValido(usuarioId);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();

            var pedidoCriado = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();

            if (pedidoCriado == null)
                throw new Exception("Não foi possivel criar o pedido no contexto");

            return pedidoCriado;
        }
        private PedidoItemCreateDTO CriarPedidoItemValido(Guid itemId, int quantidade)
        {
            return new PedidoItemCreateDTO() { ItemId = itemId, Quantidade = quantidade };
        }
        private string CriarUrl(Guid id)
        {
            return $"api/Pedidos/{id}/Itens";
        }

        [Fact]
        public async Task Deve_CriarPedidoVazio_Retorno201()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

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
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();
            
            var postBadRequest = await _client.PostAsJsonAsync(_url, pedido);

            Assert.Equal(HttpStatusCode.BadRequest, postBadRequest.StatusCode);
        }

        [Fact]
        public async Task Deve_DeletarPedido_Retorno204()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

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
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{IdTeste}");

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_FinalizarPedido_SemErros()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);
            
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);
            var pedidoItem = CriarPedidoItemValido(itemCriado.Id, 1);

            var postItemResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postItemResponse.EnsureSuccessStatusCode();

            var finishResponse = await _client.PatchAsync($"{_url}/{pedido.Id}", null);
            finishResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_FinalizarPedidoSemItem_Retorno400()
        {
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var finishResponse = await _client.PatchAsync($"{_url}/{pedido.Id}", null);

            Assert.NotNull(finishResponse);
            Assert.Equal(HttpStatusCode.BadRequest, finishResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_FinalizarPedidoDuasVezes_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);
            var pedidoItem = CriarPedidoItemValido(itemCriado.Id, 1);

            var postItemResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postItemResponse.EnsureSuccessStatusCode();

            var finishResponse = await _client.PatchAsync($"{_url}/{pedido.Id}", null);
            finishResponse.EnsureSuccessStatusCode();

            var finishResponse2 = await _client.PatchAsync($"{_url}/{pedido.Id}", null);
            Assert.NotNull(finishResponse2);
            Assert.Equal(HttpStatusCode.BadRequest, finishResponse2.StatusCode);
        }

        [Fact]
        public async Task Deve_ObterTodosPedidosDeUmUsuario_Resposta200()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido1 = await CriarPedidoNoContexto(usuarioCriado.Id);
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);
            var url = $"api/Pedidos/{pedido1.Id}/Itens";
            var pedidoItem = CriarPedidoItemValido(itemCriado.Id, 1);

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postResponse.EnsureSuccessStatusCode();

            var finishResponse = await _client.PatchAsync($"{_url}/{pedido1.Id}", null);

            var pedido2 = await CriarPedidoNoContexto(usuarioCriado.Id);

            var getPedidosUsuario = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            getPedidosUsuario.EnsureSuccessStatusCode();

            var pedidosUsuario = await getPedidosUsuario.Content.ReadFromJsonAsync < List<PedidoResponseDTO>>();

            Assert.NotNull(pedidosUsuario);
            Assert.Equal(2, pedidosUsuario.Count);
        }

        [Fact]
        public async Task Deve_ObterPedidoExistentePeloId_Resposta200()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);
            var pedido = await CriarPedidoNoContexto(usuarioCriado.Id);

            var getResponse = await _client.GetAsync($"{_url}/{pedido.Id}/Itens");
            getResponse.EnsureSuccessStatusCode();

            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_ObterPedidoInexistente_Resposta404()
        {
            var getResponse = await _client.GetAsync($"{_url}/{IdTeste}");

            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }
    }
}
