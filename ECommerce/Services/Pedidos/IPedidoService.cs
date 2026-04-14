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


    }
}
