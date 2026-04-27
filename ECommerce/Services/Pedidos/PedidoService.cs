using AutoMapper;
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
        private readonly IMapper _mapper;
        public PedidoService(IPedidosRepository repository, IUsuariosService usuarioService, IItemService itemService, IMapper mapper)
        {
            _repository = repository;
            _usuarioService = usuarioService;
            _itemService = itemService;
            _mapper = mapper;
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

            return _mapper.Map<PedidoResponseDTO>(pedido);
        }

        public async Task<PedidoResponseDTO> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);

            return _mapper.Map<PedidoResponseDTO>(pedido);
        }

        public async Task<IReadOnlyCollection<PedidoResponseDTO>> ObterTodosPedidosUsuario(Guid usuarioId)
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(usuarioId);
            var pedidos = await _repository.ObterPedidosPorIdUsuario(usuarioId);

            return _mapper.Map<IReadOnlyCollection<PedidoResponseDTO>>(pedidos);
        }

        public async Task RemoverPedido(Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            List<PedidoItem> pedidoItems = new();
            foreach(var pi in pedido.Itens)
            {
                var item = await _itemService.ObterPorId(pi.IdItem);
                item.AumentarEstoque(pi.Quantidade);
                pedidoItems.Add(pi);
            }
            await _repository.RemoverPedidoItens(pedidoItems);
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
            var item = await _itemService.ObterPorId(novoPedidoItem.ItemId);
            if (item == null)
                throw new ItemNotFoundException();

            item.ReduzirEstoque(novoPedidoItem.Quantidade);

            pedido.AdicionarNovoItem(item.Id, item.Preco, novoPedidoItem.Quantidade);
            await _repository.AdicionarItemNoPedido();
        }

        public async Task AtualizarItemNoPedido(PedidoItemUpdateDTO pedidoItemAtualizado, Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            var item = await _itemService.ObterPorId(pedidoItemAtualizado.ItemId);
            if (item == null)
                throw new ItemNotFoundException();

            var diferenca = pedido.AtualizarItem(pedidoItemAtualizado.ItemId, pedidoItemAtualizado.Quantidade);
            if (diferenca == 0)
                return;

            if (diferenca < 0)
                item.ReduzirEstoque(Math.Abs(diferenca));
            else
                item.AumentarEstoque(Math.Abs(diferenca));

            await _repository.AtualizarPedido(pedido);
        }

        public async Task RemoverItemNoPedido(Guid pedidoItemRemove, Guid pedidoId)
        {
            var pedido = await ObterPedidoPeloId(pedidoId);
            var pedidoItem = pedido.RemoverItem(pedidoItemRemove);

            var item = await _itemService.ObterPorId(pedidoItem.IdItem);
            if (item == null)
                throw new ItemNotFoundException();

            item.AumentarEstoque(pedidoItem.Quantidade);

            await _repository.AtualizarPedido(pedido);
        }

    }
}
