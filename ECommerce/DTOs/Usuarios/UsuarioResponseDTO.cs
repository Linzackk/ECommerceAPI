namespace ECommerce.DTOs.Usuarios
{
    public class UsuarioResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string NumeroCasa { get; set; }
        public string Cep { get; set; }
    }
}
