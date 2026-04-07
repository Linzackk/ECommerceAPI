namespace ECommerce.Models
{
    public class PedidoItem
    {
        public Guid IdPedido { get; private set; }
        public Pedido Pedido { get; private set; }
        public Guid IdItem { get; private set; }
        public Item Item { get; private set; }
        public int Quantidade { get; private set; }
        public string Id { get; private set; }
        public decimal ValorUnitario { get; private set; }
    }
}
