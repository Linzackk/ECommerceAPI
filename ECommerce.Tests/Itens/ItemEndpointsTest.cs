using ECommerce.DTOs.Itens;
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

        public ItemEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private ItemCreateDTO CriarItemValido()
        {
            return new ItemCreateDTO()
            {
                Nome = "Escova de Cabelo",
                Descricao = "Para escovar cabelos, feita de plástico",
                Estoque = 7,
                Preco = 8.95M
            };
        }

        private ItemCreateDTO CriarItemInvalido()
        {
            return new ItemCreateDTO()
            {
                Nome = "Teste",
                Descricao = "Teste",
                Estoque = -5,
                Preco = -50
            };
        }

        [Fact]
        public async Task Deve_CriarNovoItem_Retorno201()
        {
            var item = CriarItemValido();

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
            var item = CriarItemInvalido();
            var postResponse = await _client.PostAsJsonAsync(_url, item);

            Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }
    }
}
