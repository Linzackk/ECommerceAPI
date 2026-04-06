namespace ECommerce.Models
{
    public class Usuario
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Telefone { get; private set; }
        public string Rua { get; private set; }
        public string Cidade { get; private set; }
        public string Numero { get; private set; }
        public string Cep { get; private set; }
        public string CPF { get; private set; }
        public Login Login { get; private set; }
        public List<Pedido> Pedidos { get; private set; }
    }
}
