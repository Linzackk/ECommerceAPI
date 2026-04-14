namespace ECommerce.DTOs.Pedidos
{
    public class ItemPedidoResponseDTO
    {
        public Guid IdItem { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
