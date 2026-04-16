using ECommerce.DTOs.Itens;
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
        private readonly string _urlItem = "api/Itens";
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

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
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
        private string CriarUrl(Guid id)
        {
            return $"api/Pedidos/{id}/Itens";
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

        //[Fact]
        //public async Task Deve_FinalizarPedido_SemErros()
        //{
        //    var item = await CriarItemNoContexto();
        //    var pedido = await CriarUsuarioEPedidoNoContexto();
        //    var url = CriarUrl(pedido.Id);
        //    var pedidoItem = CriarPedidoItemValido(item.Id, 1);

        //    var postItemResponse = await _client.PostAsJsonAsync(url, pedidoItem);
        //    postItemResponse.EnsureSuccessStatusCode();

        //    var finishResponse = await _client.PatchAsync($"{_url}/{pedido.Id}", null);
        //    finishResponse.EnsureSuccessStatusCode();
        //    Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);
        //}

        //[Fact]
        //public async Task Deve_ObterTodosPedidosDeUmUsuario_Resposta200()
        //{

        //    var usuario = CriarUsuarioValido();

        //    var userPostResponse = await _client.PostAsJsonAsync(_urlUsuario, usuario);
        //    userPostResponse.EnsureSuccessStatusCode();
        //    var usuarioCriado = await userPostResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

        //    var pedido = CriarPedidoValido(usuarioCriado.Id);

        //    var postResponse = await _client.PostAsJsonAsync(_url, pedido);
        //    postResponse.EnsureSuccessStatusCode();
        //}
    }
}
