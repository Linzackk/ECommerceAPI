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
        private readonly Guid IdTeste = Guid.NewGuid();

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

        [Fact]
        public async Task Deve_CriarItem_BuscarItemRetorno200()
        {
            var item = CriarItemValido();

            var postResponse = await _client.PostAsJsonAsync(_url, item);
            postResponse.EnsureSuccessStatusCode();

            var itemCriado = await postResponse.Content.ReadFromJsonAsync<ItemResponseDTO>();

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
    }
}
