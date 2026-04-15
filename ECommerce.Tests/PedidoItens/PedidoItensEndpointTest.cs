using ECommerce.DTOs.Itens;
using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using ECommerce.Models;
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
        private readonly string _urlUsuario = "api/Usuarios";
        private readonly string _urlItem = "api/Itens";
        private readonly Guid IdTeste = Guid.NewGuid();

        public PedidoItensEndpointTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private string CriarUrl(Guid id)
        {
            return $"api/Pedidos/{id}/Itens";
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

        private ItemCreateDTO CriarItemValido()
        {
            return new ItemCreateDTO()
            {
                Nome = "Item1",
                Descricao = "Um Item",
                Estoque = 90,
                Preco = 19.82M
            };
        }
        private async Task<PedidoResponseDTO> CriarUsuarioEPedidoNoContexto()
        {
            var usuario = CriarUsuarioValido();

            var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
            userPostResponse.EnsureSuccessStatusCode();
            var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            var pedido = CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_urlPedidos, pedido);
            postResponse.EnsureSuccessStatusCode();

            var pedidoCriado = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();
            return pedidoCriado;
        }
        private async Task<ItemResponseDTO> CriarItemNoContexto()
        {
            var item = CriarItemValido();

            var postResponse = await _client.PostAsJsonAsync(_urlItem, item);
            postResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            var itemCriado = await postResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();
            return itemCriado;
        }
        private PedidoItemCreateDTO CriarPedidoItemValido(Guid itemId, int quantidade)
        {
            return new PedidoItemCreateDTO() { ItemId = itemId, Quantidade = quantidade };
        }

        [Fact]
        public async Task Deve_AdicionarItem_AoPedidoExistente_Retorno200()
        {
            var item = await CriarItemNoContexto();
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);
            var pedidoItem = CriarPedidoItemValido(item.Id, 1);

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postResponse.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_AdicionarItemInexistente_AoPedido_Retorno404()
        {
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);

            var postResponse = await _client.PostAsJsonAsync(url, IdTeste);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_RemoverItemDoPedido_Retorno200()
        {
            var item = await CriarItemNoContexto();
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);
            var pedidoItem = CriarPedidoItemValido(item.Id, 1);

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postResponse.EnsureSuccessStatusCode();

            var deleteResponse = await _client.DeleteAsync($"{url}/{item.Id}");
        }

        [Fact]
        public async Task Deve_RemoverItemInexistenteDoPedido_Retorno400()
        {
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);

            var deleteResponse = await _client.DeleteAsync($"{url}/{IdTeste}");
        }

        [Fact]
        public async Task Deve_AtualizarAQuantidadeDoItemNoPedido_Retorno200()
        {
            var item = await CriarItemNoContexto();
            var pedido = await CriarUsuarioEPedidoNoContexto();
            var url = CriarUrl(pedido.Id);
            var pedidoItem = CriarPedidoItemValido(item.Id, 1);
            var updateItem = new PedidoItemUpdateDTO() { ItemId = item.Id, Quantidade = 3 };

            var postResponse = await _client.PostAsJsonAsync(url, pedidoItem);
            postResponse.EnsureSuccessStatusCode();

            var updateResponse = await _client.PatchAsJsonAsync(url, updateItem);
            updateResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync(url);
            getResponse.EnsureSuccessStatusCode();

            var response = await getResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();
            Assert.NotNull(response);
        }
    }
}
