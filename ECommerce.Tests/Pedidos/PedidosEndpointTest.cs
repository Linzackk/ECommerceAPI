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
        private readonly PedidoItemHelper pedidoItemHelper;
        private readonly LoginHelper loginHelper;

        public PedidosEndpointTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            pedidoHelper = new PedidoHelper(_client);
            usuarioHelper = new UsuarioHelper(_client);
            itemHelper = new ItemHelper(_client);
            pedidoItemHelper = new PedidoItemHelper(_client);
            loginHelper = new LoginHelper(_client);
        }
        private async Task<ItemResponseDTO> CriarItemNoContexto()
        {
            loginHelper.RemoverTokenDoClient();
            var admin = usuarioHelper.CriarAdminValido();
            var adminCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(admin);

            var loginAdmin = loginHelper.CriarLoginEntrarValido(admin.Email, admin.Senha);
            var tokenAdmin = await loginHelper.FazerLoginCorretamente(loginAdmin);

            loginHelper.AdicionarTokenAoClient(tokenAdmin);

            var item = itemHelper.CriarItemValido();
            var itemCriado = await itemHelper.CriarItemValido_NoContexto(item);
            loginHelper.RemoverTokenDoClient();
            return itemCriado;
        }

        [Fact]
        public async Task Deve_CriarPedidoVazio_Retorno201()
        {
            loginHelper.RemoverTokenDoClient();
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_CriarPedidoComPedidoJaAberto_Retorno400()
        {
            loginHelper.RemoverTokenDoClient();
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);

            var postResponse = await _client.PostAsJsonAsync(_url, pedido);
            postResponse.EnsureSuccessStatusCode();
            
            var postBadRequest = await _client.PostAsJsonAsync(_url, pedido);

            Assert.Equal(HttpStatusCode.BadRequest, postBadRequest.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_DeletarPedido_Retorno204()
        {
            loginHelper.RemoverTokenDoClient();
            var itemCriado = await CriarItemNoContexto();

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);
            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);

            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{pedidoCriado.Id}");
            deleteResponse.EnsureSuccessStatusCode();

            var itemAtualizado = await itemHelper.BuscarItem_NoContexto(itemCriado.Id);
            Assert.Equal(itemAtualizado.Estoque, itemCriado.Estoque);

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_DeletarPedido_EstoqueItensAumentado_Retorno204()
        {

        }

        [Fact]
        public async Task Deve_DeletarPedidoInexistente_Retorno404()
        {
            loginHelper.RemoverTokenDoClient();
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var deleteResponse = await _client.DeleteAsync($"{_url}/{IdTeste}");

            Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_FinalizarPedido_SemErros()
        {
            loginHelper.RemoverTokenDoClient();
            var itemCriado = await CriarItemNoContexto();

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);

            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);
            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var finishResponse = await _client.PatchAsync($"{_url}/{pedidoCriado.Id}", null);
            finishResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);
        }

        [Fact]
        public async Task Deve_FinalizarPedidoSemItem_Retorno400()
        {
            loginHelper.RemoverTokenDoClient();
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);
            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var finishResponse = await _client.PatchAsync($"{_url}/{pedidoCriado.Id}", null);

            Assert.NotNull(finishResponse);
            Assert.Equal(HttpStatusCode.BadRequest, finishResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_FinalizarPedidoDuasVezes_Retorno400()
        {
            loginHelper.RemoverTokenDoClient();
            var itemCriado = await CriarItemNoContexto();

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);


            var url = pedidoHelper.CriarUrlPedido(pedidoCriado.Id);

            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);
            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var finishResponse = await _client.PatchAsync($"{_url}/{pedidoCriado.Id}", null);
            finishResponse.EnsureSuccessStatusCode();

            var finishResponse2 = await _client.PatchAsync($"{_url}/{pedidoCriado.Id}", null);
            Assert.NotNull(finishResponse2);
            Assert.Equal(HttpStatusCode.BadRequest, finishResponse2.StatusCode);
        }

        [Fact]
        public async Task Deve_ObterTodosPedidosDeUmUsuario_Resposta200()
        {
            loginHelper.RemoverTokenDoClient();
            var itemCriado = await CriarItemNoContexto();

            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido1 = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado1 = await pedidoHelper.CriarPedido_NoContexto(pedido1);

            var url = pedidoHelper.CriarUrlPedido(pedidoCriado1.Id);

            var pedidoItem = pedidoItemHelper.CriarPedidoItemValido(itemCriado.Id, 1);
            await pedidoItemHelper.CriarPedidoItem_NoContexto(pedidoItem, url);

            var finishResponse = await _client.PatchAsync($"{_url}/{pedidoCriado1.Id}", null);

            var pedido2 = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado2 = await pedidoHelper.CriarPedido_NoContexto(pedido2);

            var getPedidosUsuario = await _client.GetAsync($"{_url}/{usuarioCriado.Id}");
            getPedidosUsuario.EnsureSuccessStatusCode();

            var pedidosUsuario = await getPedidosUsuario.Content.ReadFromJsonAsync < List<PedidoResponseDTO>>();

            Assert.NotNull(pedidosUsuario);
            Assert.Equal(2, pedidosUsuario.Count);
        }

        [Fact]
        public async Task Deve_ObterPedidoExistentePeloId_Resposta200()
        {
            loginHelper.RemoverTokenDoClient();
            var usuario = usuarioHelper.CriarUsuarioValido();
            var usuarioCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(usuario);

            var login = loginHelper.CriarLoginEntrarValido(usuario.Email, usuario.Senha);
            var token = await loginHelper.FazerLoginCorretamente(login);

            loginHelper.AdicionarTokenAoClient(token);

            var pedido = pedidoHelper.CriarPedidoValido(usuarioCriado.Id);
            var pedidoCriado = await pedidoHelper.CriarPedido_NoContexto(pedido);

            var getResponse = await _client.GetAsync($"{_url}/{pedidoCriado.Id}/Itens");
            getResponse.EnsureSuccessStatusCode();

            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }

        [Fact]
        public async Task Deve_ObterPedidoInexistente_Resposta404()
        {
            loginHelper.RemoverTokenDoClient();
            var admin = usuarioHelper.CriarAdminValido();
            var adminCriado = await usuarioHelper.CriarUsuarioValido_NoContexto(admin);

            var loginAdmin = loginHelper.CriarLoginEntrarValido(admin.Email, admin.Senha);
            var tokenAdmin = await loginHelper.FazerLoginCorretamente(loginAdmin);

            loginHelper.AdicionarTokenAoClient(tokenAdmin);

            var getResponse = await _client.GetAsync($"{_url}/{IdTeste}");

            Assert.NotNull(getResponse);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            loginHelper.RemoverTokenDoClient();
        }
    }
}
