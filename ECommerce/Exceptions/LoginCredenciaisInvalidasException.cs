namespace ECommerce.Exceptions
{
    public class LoginCredenciaisInvalidasException : BadRequestException
    {
        public LoginCredenciaisInvalidasException()
            : base("Credenciais Inválidas.") { }
    }
}
