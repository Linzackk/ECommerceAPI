namespace ECommerce.Models
{
    public class Login
    {
        public string Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }
    }
}
