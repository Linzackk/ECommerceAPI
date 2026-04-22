using ECommerce.DTOs.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Helpers
{
    public class PedidoHelper
    {
        private readonly string _url = "api/Pedidos";
        private readonly HttpClient _client;

        public PedidoHelper(HttpClient client)
        {
            _client = client;
        }
        public PedidoCreateDTO CriarPedidoValido(Guid idUsuario)
        {
            return new PedidoCreateDTO() 
            { 
                IdUsuario = idUsuario 
            };
        }
        public PedidoItemCreateDTO CriarPedidoItemValido(Guid itemId, int quantidade)
        {
            return new PedidoItemCreateDTO() 
            { 
                ItemId = itemId, 
                Quantidade = quantidade 
            };
        }
        public string CriarUrlPedido(Guid id)
        {
            return $"api/Pedidos/{id}/Itens";
        }
        public async Task<PedidoResponseDTO> CriarPedidoNoContexto(PedidoCreateDTO novoPedido)
        {
            var postResponse = await _client.PostAsJsonAsync(_url, novoPedido);
            if (!postResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possivel criar o pedido no contexto.");

            var pedidoCriado = await postResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();

            if (pedidoCriado == null)
                throw new Exception("O Pedido foi retornado vazio.");

            return pedidoCriado;
        }

        public async Task FinalizarPedido_NoContexto(Guid pedidoId)
        {
            var finishResponse = await _client.PatchAsync($"{_url}/{pedidoId}", null);
            if (!finishResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível finalizar o Pedido.");
        }

        public async Task<PedidoResponseDTO> ObterPedido_NoContexto(Guid pedidoId)
        {
            var getResponse = await _client.GetAsync($"{_url}/{pedidoId}/Itens");
            if (!getResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível pegar informações do Pedido.");

            var pedido = await getResponse.Content.ReadFromJsonAsync<PedidoResponseDTO>();
            return pedido;
        }

        public async Task<List<PedidoResponseDTO>> ObterPedidosDoUsuario_NoContexto(Guid usuarioId)
        {
            var getResponse = await _client.GetAsync($"{_url}/{usuarioId}");
            if (!getResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível pegar os pedidos do Usuário.");

            var pedidos = await getResponse.Content.ReadFromJsonAsync<List<PedidoResponseDTO>>();
            return pedidos;
        }
    }
}
