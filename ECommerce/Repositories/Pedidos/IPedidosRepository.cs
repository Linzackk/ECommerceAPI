using ECommerce.Models;

namespace ECommerce.Repositories.Pedidos
{
    public interface IPedidosRepository
    {
        Task CriarNovoPedido(Pedido novoPedido);
        Task<Pedido?> ObterPedidoPorId(Guid pedidoId);
        Task<IReadOnlyCollection<Pedido>> ObterPedidosPorIdUsuario(Guid idUsuario);
        Task AtualizarPedido(Pedido pedido);
        Task RemoverPedido(Pedido pedido);
        Task<Pedido?> ObterPedidoAberto(Guid idUsuario);
        Task AdicionarItemNoPedido(PedidoItem pedidoItem);
    }
}
