namespace ECommerce.DTOs.Pedidos
{
    public class PedidoResponseDTO
    {
        public Guid Id { get; set; }
        public decimal ValorTotal { get; set; }
        public IReadOnlyCollection<ItemPedidoResponseDTO> Itens { get; set; }
    }
}
