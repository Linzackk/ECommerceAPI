namespace ECommerce.Exceptions
{
    public class ParametroInvalidoException : Exception
    {
        public ParametroInvalidoException(string mensagem)
            : base(mensagem) { }
    }
}
