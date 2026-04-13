namespace ECommerce.DTOs.Itens
{
    public class ItemResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Estoque { get; set; }
        public decimal Preco { get; set; }
        public DateOnly DataCriacao { get; set; }
    }
}
