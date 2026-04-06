namespace ECommerce.Models
{
    public class Item
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public int Estoque { get; private set; }
        public decimal Preco { get; private set; }
        public DateTime DataCriacao { get; private set; }
    }
}
