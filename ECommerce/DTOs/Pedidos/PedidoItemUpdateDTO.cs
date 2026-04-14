namespace ECommerce.DTOs.Pedidos
{
    public class PedidoItemUpdateDTO
    {
        public Guid PedidoId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantidade { get; set; }
    }
}
