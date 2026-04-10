namespace ECommerce.DTOs.Login
{
    public class LoginCreateDTO
    {
        public Guid IdUsuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
