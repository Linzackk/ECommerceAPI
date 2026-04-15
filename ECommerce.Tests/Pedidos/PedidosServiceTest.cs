using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Itens;
using ECommerce.Repositories.Pedidos;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Itens;
using ECommerce.Services.Pedidos;
using ECommerce.Services.Usuarios;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Pedidos
{
    public class PedidosServiceTest
    {
        private readonly Guid IdTeste = Guid.NewGuid(); 
        private PedidoCreateDTO CriarPedido()
        {
            return new PedidoCreateDTO() { IdUsuario = IdTeste };
        }

        private PedidoCreateDTO CriarPedidoInvalido()
        {
            return new PedidoCreateDTO() { IdUsuario = Guid.NewGuid() };
        }
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

        private Pedido CriarPedidoFechadoValido()
        {
            var pedido = new Pedido(IdTeste);
            pedido.AdicionarNovoItem(IdTeste, 8.95M, 5);
            pedido.FinalizarPedido();
            Console.WriteLine($"{pedido.Finalizado}");
            return pedido;
        }

        private Pedido CriarPedidoAbertoValido()
        {
            return new Pedido(IdTeste);
        }

        private PedidoCreateDTO CriarPedidoDTOValido()
        {
            return new PedidoCreateDTO() { IdUsuario = IdTeste };
        }

        [Fact]
        public async Task Deve_CriarNovoPedido_SemErros()
        {
            var usuario = CriarUsuarioValido();
            var pedidoInput = CriarPedidoDTOValido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mock.Setup(x => x.ObterPedidoAberto(IdTeste))
                .ReturnsAsync((Pedido?)null);

            mockUsuario.Setup(x => x.ObterUsuarioPorId(IdTeste))
                .ReturnsAsync(usuario);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            var resultado = await service.CriarNovoPedido(pedidoInput);

            Assert.NotNull(resultado);
            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Empty(resultado.Itens);
            mock.Verify(x => x.CriarNovoPedido(It.IsAny<Pedido>()), Times.Once);
        }

        [Fact]
        public async Task Deve_LancarErro_QuandoCriarPedidoUsuarioInexistente()
        {
            var pedidoInput = CriarPedidoInvalido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mockUsuario.Setup(x => x.ObterUsuarioPorId(pedidoInput.IdUsuario))
                .ThrowsAsync(new UsuarioNotFound());

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            await Assert.ThrowsAsync<UsuarioNotFound>(() => service.CriarNovoPedido(pedidoInput));
            mock.Verify(x => x.CriarNovoPedido(It.IsAny<Pedido>()), Times.Never);
        }

        [Fact]
        public async Task Deve_ObterTodosPedidos_SemErros()
        {
            var usuario = CriarUsuarioValido();
            var pedidos = new List<Pedido>() { CriarPedidoAbertoValido(), CriarPedidoFechadoValido() };

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mock.Setup(x => x.ObterPedidosPorIdUsuario(IdTeste))
                .ReturnsAsync(pedidos);

            mockUsuario.Setup(x => x.ObterUsuarioPorId(IdTeste))
                .ReturnsAsync(usuario);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            var resultado = await service.ObterTodosPedidosUsuario(IdTeste);

            Assert.NotNull(resultado);
            Assert.Equal(typeof(List<PedidoResponseDTO>), resultado.GetType());
            Assert.Equal(2, resultado.Count);
            mock.Verify(x => x.ObterPedidosPorIdUsuario(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Deve_ObterPedido_SemErros()
        {
            var usuario = CriarUsuarioValido();
            var pedidoResponse = CriarPedidoAbertoValido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mock.Setup(x => x.ObterPedidoPorId(IdTeste))
                .ReturnsAsync(pedidoResponse);

            mockUsuario.Setup(x => x.ObterUsuarioPorId(IdTeste))
                .ReturnsAsync(usuario);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            var resultado = await service.ObterPedidoPorId(IdTeste);

            Assert.NotNull(resultado);
            mock.Verify(x => x.ObterPedidoPorId(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Deve_LancarErro_QuandoProcurarPedidoInexistente()
        {
            var usuario = CriarUsuarioValido();

            var mock = new Mock<IPedidosRepository>();
            var mockUsuario = new Mock<IUsuariosService>();
            var mockItem = new Mock<IItemService>();

            mockUsuario.Setup(x => x.ObterUsuarioPorId(IdTeste))
                .ReturnsAsync(usuario);

            var service = new PedidoService(mock.Object, mockUsuario.Object, mockItem.Object);

            await Assert.ThrowsAsync<PedidoNotFound>(() => service.ObterPedidoPorId(IdTeste));
            mock.Verify(x => x.ObterPedidoPorId(It.IsAny<Guid>()), Times.Once);
        }
    }
}
