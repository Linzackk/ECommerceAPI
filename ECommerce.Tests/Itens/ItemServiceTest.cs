using ECommerce.DTOs.Itens;
using ECommerce.Models;
using ECommerce.Repositories.Itens;
using ECommerce.Services.Itens;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Itens
{
    public class ItemServiceTest
    {
        static Guid IdTeste = Guid.NewGuid();
        static string NomeTeste = "Escova de Cabelo";
        static string DescricaoTeste = "Serve para escovar o cabelo, é feito de plástico";
        static int EstoqueTeste = 17;
        static decimal PrecoTeste = 9.95M;
        private Item CriarItemValido()
        {
            return new Item(NomeTeste, DescricaoTeste, EstoqueTeste, PrecoTeste);
        }
        private ItemCreateDTO CriarItemDTOValido()
        {
            return new ItemCreateDTO()
            {
                Nome = NomeTeste,
                Descricao = DescricaoTeste,
                Estoque = EstoqueTeste,
                Preco = PrecoTeste
            };
        }

        [Fact]
        public async Task DeveCriarItem_DeveRetornarItemResponse()
        {
            var item = CriarItemValido();
            var itemCreate = CriarItemDTOValido();

            var mock = new Mock<IItemRepository>();
            mock.Setup(x => x.ObterItemPorId(IdTeste))
                .ReturnsAsync(item);

            var service = new ItemService(mock.Object);

            var resultado = await service.ObterItemPorId(IdTeste);

            Assert.NotNull(resultado);
            Assert.Equal(NomeTeste, resultado.Nome);
            Assert.Equal(DescricaoTeste, resultado.Descricao);
            Assert.Equal(EstoqueTeste, resultado.Estoque);
            Assert.Equal(PrecoTeste, resultado.Preco);
        }
    }
}
