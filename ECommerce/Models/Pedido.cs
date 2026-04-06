namespace ECommerce.Models
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public Guid IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }
        public bool Finalizado { get; private set; }

        public List<PedidoItem> Itens { get; private set; } = new();
    }
}
