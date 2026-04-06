namespace ECommerce.Models
{
    public class Pedido
    {
        public string Id { get; private set; }
        public string IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }
        public decimal Total { get; private set; }
        public bool Finalizado { get; private set; }
    }
}
