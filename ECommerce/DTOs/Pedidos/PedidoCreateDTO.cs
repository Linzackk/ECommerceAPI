namespace ECommerce.DTOs.Pedidos
{
    public class PedidoCreateDTO
    {
        public Guid IdUsuario { get; set; }
        public IReadOnlyCollection<Guid>? IdsItens { get; set; }

    }
}
