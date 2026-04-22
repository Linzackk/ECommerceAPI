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

        public async Task CriarPedidoItem_NoContexto(PedidoItemCreateDTO novoPedidoItem, string url)
        {
            var postResponse = await _client.PostAsJsonAsync(url, novoPedidoItem);
            if (!postResponse.IsSuccessStatusCode)
                throw new Exception("Não foi possível adicionar o PedidoItem no Contexto.");
        }
    }
}
