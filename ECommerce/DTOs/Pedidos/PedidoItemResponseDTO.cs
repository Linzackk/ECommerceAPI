namespace ECommerce.DTOs.Pedidos
{
    public class PedidoItemResponseDTO
    {
        public Guid IdItem { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
