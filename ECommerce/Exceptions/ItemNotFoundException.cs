namespace ECommerce.Exceptions
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException()
            : base("Item não encontrado") { }
    }
}
