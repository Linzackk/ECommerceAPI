namespace ECommerce.Models
{
    public class PedidoItem
    {
        public string IdPedido { get; private set; }
        public Pedido Pedido { get; private set; }
        public string IdItem { get; private set; }
        public Item Item { get; private set; }
        public int Quantidade { get; private set; }
        public string Id { get; private set; }
        public decimal ValorUnitario { get; private set; }
    }
}
