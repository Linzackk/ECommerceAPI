using ECommerce.DTOs.Itens;
using ECommerce.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Itens
{
    public class ItemEndpointsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _url = "api/itens";
        private readonly HttpClient _client;
        private readonly Guid IdTeste = Guid.NewGuid();
        private readonly ItemHelper itemHelper;
        private readonly UsuarioHelper usuarioHelper;
        private readonly PedidoHelper pedidoHelper;
        private readonly PedidoItemHelper pedidoItemHelper;

        public ItemEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            itemHelper = new ItemHelper(_client);
            usuarioHelper = new UsuarioHelper(_client);
            pedidoHelper = new PedidoHelper(_client);
            pedidoItemHelper = new PedidoItemHelper(_client);
        }

        [Fact]
        public async Task Deve_CriarNovoItem_Retorno201()
        {
            var item = itemHelper.CriarItemValido();

            var postResponse = await _client.PostAsJsonAsync(_url, item);
            postResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var itemCriado = await postResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();

            Assert.NotNull(itemCriado);
            Assert.Equal(item.Nome, itemCriado.Nome);
            Assert.Equal(item.Descricao, itemCriado.Descricao);
            Assert.Equal(item.Estoque, itemCriado.Estoque);
            Assert.Equal(item.Preco, itemCriado.Preco);
            Assert.NotEqual(Guid.Empty, itemCriado.Id);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), itemCriado.DataCriacao);
        }

        [Fact]
        public async Task Deve_CriarNovoItem_Retorno400()
        {
            var item = itemHelper.CriarItemInvalido();
            var postResponse = await _client.PostAsJsonAsync(_url, item);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_CriarItem_BuscarItemRetorno200()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var getResponse = await _client.GetAsync($"{_url}/{itemCriado.Id}");
            getResponse.EnsureSuccessStatusCode();

            var itemResponse = await getResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();

            Assert.NotNull(itemCriado);
            Assert.Equal(item.Nome, itemResponse.Nome);
            Assert.Equal(item.Descricao, itemResponse.Descricao);
            Assert.Equal(item.Estoque, itemResponse.Estoque);
            Assert.Equal(item.Preco, itemResponse.Preco);
            Assert.NotEqual(Guid.Empty, itemResponse.Id);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), itemResponse.DataCriacao);
        }

        [Fact]
        public async Task Deve_ProcurarItemInexistente_Retorno404()
        {
            var getResponse = await _client.GetAsync($"{_url}/{IdTeste}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AtualizarItemCorretamente_Retorno204()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var atualizacao = itemHelper.CriarAtualizacaoValida();

            var estoqueAtualizado = itemCriado.Estoque + atualizacao.EstoqueNovo;

            var patchResponse = await _client.PatchAsJsonAsync($"{_url}/{itemCriado.Id}", atualizacao);
            patchResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

            var getResponse = await _client.GetAsync($"{_url}/{itemCriado.Id}");
            getResponse.EnsureSuccessStatusCode();

            var itemResponse = await getResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();

            Assert.NotNull(itemResponse);
            Assert.Equal(item.Nome, itemResponse.Nome);
            Assert.Equal(item.Descricao, itemResponse.Descricao);
            Assert.Equal(estoqueAtualizado, itemResponse.Estoque);
            Assert.Equal(atualizacao.Preco, itemResponse.Preco);
            Assert.NotEqual(Guid.Empty, itemResponse.Id);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), itemResponse.DataCriacao);
        }

        [Fact]
        public async Task Deve_AtualizarItem_LancarExcecaoComInformacoesVazias()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var atualizacao = new ItemUpdateDTO();

            var patchResponse = await _client.PatchAsJsonAsync($"{_url}/{itemCriado.Id}", atualizacao);
            Assert.Equal(HttpStatusCode.BadRequest, patchResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_RemoverItemExistente_Retorno204()
        {
            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{itemCriado.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_RemoverItemInexistente_Retorno404()
        {
            var deleteResponse = await _client.DeleteAsync($"{_url}/{IdTeste}");
            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
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
