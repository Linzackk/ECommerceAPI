using ECommerce.DTOs.Itens;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Helpers
{
    public class ItemHelper
    {
        private readonly HttpClient _client;
        private readonly string _url = "api/Itens";

        public ItemHelper(HttpClient client)
        {
            _client = client;
        }
        public ItemCreateDTO CriarItemValido()
        {
            return new ItemCreateDTO()
            {
                Nome = "Escova de Cabelo",
                Descricao = "Para escovar cabelos, feita de plástico",
                Estoque = 7,
                Preco = 8.95M
            };
        }
        public ItemCreateDTO CriarItemInvalido()
        {
            return new ItemCreateDTO()
            {
                Nome = "Teste",
                Descricao = "Teste",
                Estoque = -5,
                Preco = -50
            };
        }

        public ItemUpdateDTO CriarAtualizacaoValida()
        {
            return new ItemUpdateDTO()
            {
                Preco = 9.85M,
                EstoqueNovo = 10
            };
        }

        public async Task<ItemResponseDTO> CriarItemValido_NoContexto(ItemCreateDTO itemValido)
        {
            var postResponse = await _client.PostAsJsonAsync(_url, itemValido);
            
            if (!postResponse.IsSuccessStatusCode)
                throw new Exception("Item não foi criado corretamente, verifique as informações.");

            var itemCriado = await postResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();
            if (itemCriado == null)
                throw new Exception("Item foi recebido nulo");

            return itemCriado;
        }

        public async Task<ItemResponseDTO> BuscarItem_NoContexto(Guid itemId)
        {
            var getResponse = await _client.GetAsync($"{_url}/{itemId}");
            if (!getResponse.IsSuccessStatusCode)
                throw new Exception("A busca não teve sucesso.");

            var itemResponse = await getResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();
            if (itemResponse == null)
                throw new Exception("O Item retornado está vazio.");

            return itemResponse;
        }

        public ItemUpdateDTO CriarAtualizacaoItemValido(int? novoEstoque, decimal? novoPreco)
        {
            if (novoEstoque == null && novoPreco == null)
                throw new Exception("Ao menos uma informação deve ser inserida para atualizar.");

            return new ItemUpdateDTO()
            {
                EstoqueNovo = novoEstoque,
                Preco = novoPreco
            };
        }

        public async Task AtualizarItem_NoContexto(ItemUpdateDTO novasInformacoes, Guid itemId)
        {
            var patchResponse = await _client.PatchAsJsonAsync($"{_url}/{itemId}", novasInformacoes);
            if (!patchResponse.IsSuccessStatusCode)
                throw new Exception("Item não foi possivel atualizar o item.");
        }

        public async Task RemoverItem_NoContexto(Guid itemId)
        {
            var deleteResponse = await _client.DeleteAsync($"{_url}/{itemId}");
            if (!deleteResponse.IsSuccessStatusCode)
                throw new Exception("Item não foi possivel atualizar o item.");

        }
    }
}
