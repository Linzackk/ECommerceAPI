using ECommerce.DTOs.Itens;
using ECommerce.Exceptions;
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
        static DateOnly Hoje = DateOnly.FromDateTime(DateTime.Now);
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
        public async Task DeveProcurarItem_DeveRetornarItemResponse()
        {
            var item = CriarItemValido();

            var mock = new Mock<IItemRepository>();
            mock.Setup(x => x.ObterItemPorId(IdTeste))
                .ReturnsAsync(item);

            var service = new ItemService(mock.Object);

            var resultado = await service.ObterItemPorId(IdTeste);

            Assert.NotNull(resultado);
            mock.Verify(x => x.ObterItemPorId(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeveCriarItem_ItemCriadoComSucesso()
        {
            var itemCreate = CriarItemDTOValido();

            var mock = new Mock<IItemRepository>();

            var service = new ItemService(mock.Object);

            var resultado = await service.CriarNovoItem(itemCreate);

            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal(Hoje, resultado.DataCriacao);
            mock.Verify(x => x.CriarItem(It.IsAny<Item>()), Times.Once);
        }

        [Fact]
        public async Task DeveCriarItemInvalido_ErroDeveSerLancado()
        {
            var itemInvalido = new ItemCreateDTO();

            var mock = new Mock<IItemRepository>();

            var service = new ItemService(mock.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.CriarNovoItem(itemInvalido));
            mock.Verify(x => x.CriarItem(It.IsAny<Item>()), Times.Never);
        }

        [Fact]
        public async Task DeveAtualizarItem_ErroDeveSerLancado()
        {
            var atualizacaoInvalida = new ItemUpdateDTO();

            var mock = new Mock<IItemRepository>();

            var service = new ItemService(mock.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.AtualizarItem(atualizacaoInvalida, IdTeste));
            mock.Verify(x => x.ObterItemPorId(It.IsAny<Guid>()), Times.Never);
            mock.Verify(x => x.AtualizarItem(It.IsAny<Item>()), Times.Never);
        }

        [Fact]
        public async Task DeveAtualizarItemCorretamente_RepositoryChamadoUmaVez()
        {
            var atualizacaoValida = new ItemUpdateDTO() { Preco = 5.89M };

            var item = CriarItemValido();

            var mock = new Mock<IItemRepository>();
            mock.Setup(x => x.ObterItemPorId(IdTeste))
                .ReturnsAsync(item);

            var service = new ItemService(mock.Object);

            await service.AtualizarItem(atualizacaoValida, IdTeste);
            mock.Verify(x => x.ObterItemPorId(It.IsAny<Guid>()), Times.Once);
            mock.Verify(x => x.AtualizarItem(It.IsAny<Item>()), Times.Once);
        }
    }
}
