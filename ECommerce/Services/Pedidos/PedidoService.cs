using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Pedidos;
using ECommerce.Services.Usuarios;

namespace ECommerce.Services.Pedidos
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidosRepository _repository;
        private readonly IUsuariosService _usuarioService;
        public PedidoService(IPedidosRepository repository)
        {
            _repository = repository;
        }

        private PedidoResponseDTO CriarPedidoResponse(Pedido pedidoModel)
        {
            var itensFormatados = new List<PedidoItemResponseDTO>();
            foreach(var pedidoItem in pedidoModel.Itens)
            {
                itensFormatados.Add(CriarItemPedidoResponse(pedidoItem));
            }

            return new PedidoResponseDTO()
            {
                Id = pedidoModel.Id,
                Itens = itensFormatados,
                ValorTotal = pedidoModel.CalcularTotal()
            };
        }

        private PedidoItemResponseDTO CriarItemPedidoResponse(PedidoItem pedidoItemModel)
        {
            return new PedidoItemResponseDTO()
            {
                IdItem = pedidoItemModel.Id,
                Nome = pedidoItemModel.Item?.Nome ?? string.Empty,
                Preco = pedidoItemModel.ValorUnitario,
                Quantidade = pedidoItemModel.Quantidade
            };
        }
        private async Task ValidarExistenciaUsuario(Guid idUsuario)
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(idUsuario);
        }

        public  async Task<PedidoResponseDTO> CriarNovoPedido(PedidoCreateDTO novoPedido)
        {
            await ValidarExistenciaUsuario(novoPedido.IdUsuario);

            var pedidoAberto = await _repository.ObterPedidoAberto(novoPedido.IdUsuario);
            if (pedidoAberto == null)
                throw new ParametroInvalidoException("Você já possui um pedido aberto.");

            var pedido = new Pedido(novoPedido.IdUsuario);
            await _repository.CriarNovoPedido(pedido);

            return CriarPedidoResponse(pedido);
        }

        public async Task<PedidoResponseDTO> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await _repository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
                throw new PedidoNotFound();

            return CriarPedidoResponse(pedido);
        }

        public async Task<IReadOnlyCollection<PedidoResponseDTO>> ObterTodosPedidosUsuario(Guid usuarioId)
        {
            var pedidos = await _repository.ObterPedidosPorIdUsuario(usuarioId);
            var pedidosReponse = new List<PedidoResponseDTO>();

            foreach (var pedido in pedidos)
                pedidosReponse.Add(CriarPedidoResponse(pedido));

            return pedidosReponse;
        }

        public async Task RemoverPedido(Guid pedidoId)
        {
            var pedido = await _repository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
                throw new PedidoNotFound();

            await _repository.RemoverPedido(pedido);
        }

        public async Task FinalizarPedido(Guid pedidoId)
        {
            var pedido = await _repository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
                throw new PedidoNotFound();

            pedido.FinalizarPedido();
            await _repository.AtualizarPedido(pedido);
        }
    }
}
