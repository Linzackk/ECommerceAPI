namespace ECommerce.DTOs.Pedidos
{
    public class PedidoItemCreateDTO
    {
        public Guid PedidoId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantidade { get; set; }
    }
}
