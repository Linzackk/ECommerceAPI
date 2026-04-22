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

namespace ECommerce.Tests.PedidoItens
{
    public class PedidoItensEndpointTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private readonly string _urlPedidos = "api/Pedidos";

        private readonly UsuarioHelper usuarioHelper;
        private readonly PedidoHelper pedidoHelper;
        private readonly ItemHelper itemHelper;
        private readonly PedidoItemHelper pedidoItemHelper;

        private readonly Guid IdTeste = Guid.NewGuid();

        public PedidoItensEndpointTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            pedidoItemHelper = new PedidoItemHelper(_client);
            pedidoHelper = new PedidoHelper(_client);
            usuarioHelper = new UsuarioHelper(_client);
            itemHelper = new ItemHelper(_client);
        }

        [Fact]
        public async Task Deve_AdicionarItem_AoPedidoExistente_Retorno200()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AdicionarItemInexistente_AoPedido_Retorno404()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(url, IdTeste);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_LancarErro_QuandoAdicionarItemAoPedidoFinalizado_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);

            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            var item2 = itemHelper.CriarItemValido();
            var itemCriado2 = await itemHelper.CriarItemValido_NoContexto(item);

            var pedidoItem2 = pedidoItemHelper.CriarPedidoItemValido(itemCriado2.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var urlPedido = $"api/Pedidos/{pedidoCriado.Id}";

            await pedidoHelper.FinalizarPedido_NoContexto(pedidoCriado.Id);

            var newPostResponse = await _client.PostAsJsonAsync(url, pedidoItem2);
            Assert.NotNull(newPostResponse);
            Assert.Equal(HttpStatusCode.BadRequest, newPostResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AdicionarPedidoItemComQuantidadeAcimaEstoqueItem_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 15);

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            Assert.Equal(postResponse.StatusCode, HttpStatusCode.BadRequest);

            var itemAtualizado = await itemHelper.BuscarItem_NoContexto(itemCriado.Id);
            Assert.Equal(itemCriado.Estoque, itemAtualizado.Estoque);
        }

        [Fact]
        public async Task Deve_RemoverItemDoPedido_Retorno200()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var deleteResponse = await _client.DeleteAsync($"{url}/{itemCriado.Id}");
            Assert.NotNull(deleteResponse);
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_RemoverItemPedidoFinalizado_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            await pedidoHelper.FinalizarPedido_NoContexto(pedidoCriado.Id);

            var deleteResponse = await _client.DeleteAsync($"{url}/{itemCriado.Id}");
            Assert.NotNull(deleteResponse);
            Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_RemoverItemInexistenteDoPedido_Retorno404()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);

            var deleteResponse = await _client.DeleteAsync($"{url}/{IdTeste}");

            Assert.NotNull(deleteResponse);
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AtualizarAQuantidadeDoItemNoPedido_Retorno200()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var updateItem = pedidoItemHelper.CriarPedidoItemAtualizacaoValida(itemCriado.Id, 2);

            var updateResponse = await _client.PatchAsJsonAsync(url, updateItem);
            updateResponse.EnsureSuccessStatusCode();

            var pedidoInfos = await pedidoHelper.ObterPedido_NoContexto(pedidoCriado.Id);

            Assert.NotNull(pedidoInfos);
        }

        [Fact]
        public async Task Deve_AtualizarQuantidadePedidoItem_ItemSemEstoque_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var updateItem = pedidoItemHelper.CriarPedidoItemAtualizacaoValida(itemCriado.Id, 12);

            var patchResponse = await _client.PatchAsJsonAsync(url, updateItem);
            Assert.Equal(patchResponse.StatusCode, HttpStatusCode.BadRequest);

            var estoqueAtualizado = itemCriado.Estoque - pedidoItem.Quantidade;
            var itemAtualizado = await itemHelper.BuscarItem_NoContexto(itemCriado.Id);
            Assert.Equal(itemAtualizado.Estoque, estoqueAtualizado);
        }

        [Fact]
        public async Task Deve_AtualizarItemNoPedidoFechado_Retorno400()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var updateItem = pedidoItemHelper.CriarPedidoItemAtualizacaoValida(itemCriado.Id, 2);

            await pedidoHelper.FinalizarPedido_NoContexto(pedidoCriado.Id);

            var updateResponse = await _client.PatchAsJsonAsync(url, updateItem);
            Assert.NotNull(updateResponse);
            Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AtualizarItemInexistenteNoPedido_Retorno404()
        {
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(IdTeste, 1);

            var updateResponse = await _client.PatchAsJsonAsync(url, pedidoItem);

            Assert.NotNull(updateResponse);
            Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
        }


        [Fact]
        public async Task Deve_AtualizarCorretamenteEstoqueDoItem_AoAdicionarItemAUmpedido()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var itemAtualizado = await itemHelper.BuscarItem_NoContexto(itemCriado.Id);

            int estoqueAtualizado = itemCriado.Estoque - pedidoItem.Quantidade;

            Assert.NotEqual(itemCriado.Estoque, itemAtualizado.Estoque);
            Assert.Equal(itemAtualizado.Estoque, estoqueAtualizado);
        }

        [Fact]
        public async Task Deve_AtualizarCorretamenteEstoqueDoItem_AoAtualizarQuantidadeDePedidoItem()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 5);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var pedidoAtualizado = pedidoItemHelper.CriarPedidoItemAtualizacaoValida(itemCriado.Id, 3);

            await pedidoItemHelper.AtualizarPedidoItem_NoContexto(pedidoAtualizado, pedidoCriado.Id);

            var itemAtualizado = await itemHelper.BuscarItem_NoContexto(itemCriado.Id);

            int estoqueAtualizado = itemCriado.Estoque - pedidoAtualizado.Quantidade;

            Assert.NotEqual(itemCriado.Estoque, itemAtualizado.Estoque);
            Assert.Equal(itemAtualizado.Estoque, estoqueAtualizado);
        }
    }
}
