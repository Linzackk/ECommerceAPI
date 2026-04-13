namespace ECommerce.Exceptions
{
    public class ParametroInvalidoException : BadRequestException
    {
        public ParametroInvalidoException(string mensagem)
            : base(mensagem) { }
    }
}
