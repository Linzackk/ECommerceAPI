using ECommerce.DTOs.Itens;
using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Pedidos;
using ECommerce.Services.Itens;
using ECommerce.Services.Pedidos;
using ECommerce.Services.Usuarios;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ECommerce.Tests.PedidoItens
{
    public class PedidoItensService
    {
        private readonly Guid IdTeste = Guid.NewGuid();
        private UsuarioResponseDTO CriarUsuarioValido()
        {
            return new UsuarioResponseDTO()
            {
                Id = IdTeste,
                Nome = "Teste",
                Email = "teste@teste.com",
                Cidade = "Teste",
                Rua = "Teste",
                Cep = "00000000",
                NumeroCasa = "400",
                Telefone = "11999999999"
            };
        }
        private void AdicionarItemAoPedido(Pedido pedido, Item novoItem, int quantidade)
        {
            pedido.AdicionarNovoItem(novoItem.Id, novoItem.Preco, quantidade);
        }
        private Pedido CriarPedidoFechadoValido()
        {
            var pedido = new Pedido(IdTeste);
            var id = Guid.NewGuid();
            AdicionarItemAoPedido(pedido, CriarItemValido(Guid.NewGuid()), 1);
            pedido.FinalizarPedido();
            return pedido;
        }

        private PedidoItemCreateDTO CriarItemCreateDTOValido(Guid id)
        {
            return new PedidoItemCreateDTO()
            {
                ItemId = id,
                Quantidade = 8
            };
        }

        private PedidoItemCreateDTO CriarItemCreateDTOInvalido(Guid id)
        {
            return new PedidoItemCreateDTO()
            {
                ItemId = id,
                Quantidade = -5
            };
        }

        private Item CriarItemValido(Guid itemId)
        {
            return new Item("Teste", "Teste", 5, 8.95M);
        }

        private ItemResponseDTO CriarResponseItemValido()
        {
            return new ItemResponseDTO()
            {
                Id = IdTeste,
                Nome = "Teste",
                Descricao = "Teste",
                Preco = 8.95m,
                Estoque = 8,
                DataCriacao = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        private Pedido CriarPedidoAbertoValido()
        {
            return new Pedido(IdTeste);
        }

        private PedidoItemCreateDTO CriarPedidoItemCreateValido()
        {
            return new PedidoItemCreateDTO() { ItemId = IdTeste, Quantidade = 1 };
        }

        [Fact]
        public async Task Deve_AdicionarItemAoPedido_SemErros()
        {
            var item = CriarItemCreateDTOValido(IdTeste);
            var itemResponse = CriarResponseItemValido();
            var pedido = CriarPedidoAbertoValido();
            var pedidoItem = CriarPedidoItemCreateValido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mock.Setup(x => x.ObterPedidoPorId(IdTeste))
                .ReturnsAsync(pedido);

            mockItem.Setup(x => x.ObterItemPorId(IdTeste))
                .ReturnsAsync(itemResponse);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            await service.AdicionarItemNoPedido(item, IdTeste);
        }

        [Fact]
        public async Task Deve_LancarErro_AdicionarItemInvalidoAoPedido()
        {
            var item = CriarItemCreateDTOInvalido(IdTeste);
            var itemResponse = CriarResponseItemValido();
            var pedido = CriarPedidoAbertoValido();
            var pedidoItem = CriarPedidoItemCreateValido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mock.Setup(x => x.ObterPedidoPorId(IdTeste))
                .ReturnsAsync(pedido);

            mockItem.Setup(x => x.ObterItemPorId(IdTeste))
                .ReturnsAsync(itemResponse);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.AdicionarItemNoPedido(item, IdTeste));
            mock.Verify(x => x.AdicionarItemNoPedido(It.IsAny<PedidoItem>()), Times.Never);
        }

        
    }
}
