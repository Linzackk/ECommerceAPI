namespace ECommerce.Exceptions
{
    public class UsuarioNotFound : Exception
    {
        public UsuarioNotFound()
            : base("Usuário não encontrado.") { }
    }
}
