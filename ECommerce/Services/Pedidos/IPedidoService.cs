using ECommerce.DTOs.Pedidos;

namespace ECommerce.Services.Pedidos
{
    public interface IPedidoService
    {
        Task<PedidoResponseDTO> CriarNovoPedido(PedidoCreateDTO novoPedido);
        Task<PedidoResponseDTO> ObterPedidoPorId(Guid pedidoId);
        Task<IReadOnlyCollection<PedidoResponseDTO>> ObterTodosPedidosUsuario(Guid usuarioId);
        Task FinalizarPedido(Guid pedidoId);
        Task RemoverPedido(Guid pedidoId);

        Task AdicionarItemNoPedido(PedidoItemCreateDTO novoPedidoItem, Guid pedidoId);
        Task AtualizarItemNoPedido(PedidoItemUpdateDTO pedidoItemAtualizado, Guid pedidoId);
        Task RemoverItemNoPedido(PedidoItemRemoveDTO pedidoItem, Guid pedidoId);
    }
}
