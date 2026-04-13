namespace ECommerce.DTOs.Itens
{
    public class ItemCreateDTO
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Estoque { get; set; }
        public decimal Preco { get; set; }
    }
}
