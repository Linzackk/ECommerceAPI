namespace ECommerce.Exceptions
{
    public class ItemNotFound : NotFoundException
    {
        public ItemNotFound()
            : base("Item não encontrado") { }
    }
}
