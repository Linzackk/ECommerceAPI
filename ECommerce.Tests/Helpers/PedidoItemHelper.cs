using ECommerce.DTOs.Pedidos;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Helpers
{
    public class PedidoItemHelper
    {
        private readonly HttpClient _client;
        public PedidoItemHelper(HttpClient client)
        {
            _client = client;
        }

        public PedidoItemCreateDTO CriarPedidoItemValido(Guid itemId, int quantidade)
        {
            return new PedidoItemCreateDTO()
            {
                ItemId = itemId,
                Quantidade = quantidade
            };
        }
        public PedidoItemUpdateDTO CriarPedidoItemAtualizacaoValida(Guid itemId, int quantidade)
        {
            return new PedidoItemUpdateDTO()
            {
                ItemId = itemId,
                Quantidade = quantidade
            };
        }
        public async Task CriarPedidoItem_NoContexto(PedidoItemCreateDTO novoPedidoItem, string url)
        {
            var postResponse = await _client.PostAsJsonAsync(url, novoPedidoItem);
            if (!postResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível adicionar o PedidoItem no Contexto.");
        }
        public async Task AtualizarPedidoItem_NoContexto(PedidoItemUpdateDTO pedidoItemAtualizado, Guid pedidoId)
        {
            string url = $"api/Pedidos/{pedidoId}/Itens";
            var patchResposne = await _client.PatchAsJsonAsync(url, pedidoItemAtualizado);
            if (!patchResposne.IsSuccessStatusCode)
                throw new Exception("Não foi possivel atualizar o item no pedido.");
        }

        public async Task RemoverPedidoItem_NoContexto(Guid itemId, Guid pedidoId)
        {
            string url = $"api/Pedidos/{pedidoId}/Itens/{itemId}";
            var deleteResponse = await _client.DeleteAsync(url);
            if (!deleteResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível remover o item do pedido");
        }
        
    }
}
