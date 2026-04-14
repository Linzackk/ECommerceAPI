namespace ECommerce.Exceptions
{
    public class PedidoNotFound : NotFoundException
    {
        public PedidoNotFound()
            : base("Pedido não foi encontrado") { }
    }
}
