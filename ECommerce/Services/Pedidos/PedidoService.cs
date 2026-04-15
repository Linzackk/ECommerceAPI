using ECommerce.DTOs.Pedidos;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Pedidos;
using ECommerce.Services.Itens;
using ECommerce.Services.Usuarios;

namespace ECommerce.Services.Pedidos
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidosRepository _repository;
        private readonly IUsuariosService _usuarioService;
        private readonly IItemService _itemService;
        public PedidoService(IPedidosRepository repository, IUsuariosService usuarioService, IItemService itemService)
        {
            _repository = repository;
            _usuarioService = usuarioService;
            _itemService = itemService;
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
            if (pedidoAberto != null)
                throw new ParametroInvalidoException("Você já possui um pedido aberto.");

            var pedido = new Pedido(novoPedido.IdUsuario);
            await _repository.CriarNovoPedido(pedido);

            return CriarPedidoResponse(pedido);
        }

        public async Task<PedidoResponseDTO> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);

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
            var pedido = await ObterPedidoPeloId(pedidoId);

            // Deletar todos PedidoItem que contenham o ID do Pedido
            await _repository.RemoverPedido(pedido);
        }

        public async Task FinalizarPedido(Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);

            pedido.FinalizarPedido();
            await _repository.AtualizarPedido(pedido);
        }

        private async Task<Pedido> ObterPedidoPeloId(Guid pedidoId)
        {
            var pedido = await _repository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
                throw new PedidoNotFound();

            return pedido;
        }

        public async Task AdicionarItemNoPedido(PedidoItemCreateDTO novoPedidoItem, Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            var item = await _itemService.ObterItemPorId(novoPedidoItem.ItemId);
            if (item == null)
                throw new ItemNotFoundException();

            var pedidoItem = new PedidoItem(pedido.Id, item.Id, novoPedidoItem.Quantidade, item.Preco);
            await _repository.AdicionarItemNoPedido(pedidoItem);
        }

        public async Task AtualizarItemNoPedido(PedidoItemUpdateDTO pedidoItemAtualizado, Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            pedido.AtualizarItem(pedidoItemAtualizado.ItemId, pedidoItemAtualizado.Quantidade);

            await _repository.AtualizarPedido(pedido);
        }

        public async Task RemoverItemNoPedido(Guid pedidoItemRemove, Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            pedido.RemoverItem(pedidoItemRemove);

            await _repository.AtualizarPedido(pedido);
        }

    }
}
