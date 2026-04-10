namespace ECommerce.Exceptions
{
    public class UsuarioNotFound : NotFoundException
    {
        public UsuarioNotFound()
            : base("Usuário não encontrado.") { }
    }
}
