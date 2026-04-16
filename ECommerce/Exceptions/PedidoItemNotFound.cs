namespace ECommerce.Exceptions
{
    public class PedidoItemNotFound : NotFoundException
    {
        public PedidoItemNotFound()
            : base("Esse item não foi encontrado no pedido.") { }
    }
}
