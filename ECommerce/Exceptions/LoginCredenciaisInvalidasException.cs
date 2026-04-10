namespace ECommerce.Exceptions
{
    public class LoginCredenciaisInvalidasException : Exception
    {
        public LoginCredenciaisInvalidasException()
            : base("Credenciais Inválidas.") { }
    }
}
