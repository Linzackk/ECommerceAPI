using ECommerce.Exceptions;

namespace ECommerce.Models
{
    public class PedidoItem
    {
        public Guid IdPedido { get; private set; }
        public Pedido? Pedido { get; private set; }
        public Guid IdItem { get; private set; }
        public Item? Item { get; private set; }
        public int Quantidade { get; private set; }
        public Guid Id { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public PedidoItem(Guid idPedido, Guid idItem, int quantidade, decimal valorUnitario)
        {
            if (idPedido.Equals(Guid.Empty))
                throw new ParametroInvalidoException("ID do pedido não pode ser vazio.");

            if (idItem.Equals(Guid.Empty))
                throw new ParametroInvalidoException("ID do item não pode ser vazio.");

            if (quantidade <= 0)
                throw new ParametroInvalidoException("Quantidade deve ser maior que 0.");

            if (valorUnitario <= 0)
                throw new ParametroInvalidoException("Valor unitário deve ser maior que 0.");

            IdPedido = idPedido;
            IdItem = idItem;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public void  DefinirQuantidade(int novaQuantidade)
        {
            if (novaQuantidade <= 0)
                throw new ParametroInvalidoException("Quantidade deve ser maior que 0.");

            Quantidade = novaQuantidade;
        }

        public decimal CalcularTotal()
        {
            return ValorUnitario * Quantidade;
        }

        public void SetarPedido(Pedido pedido)
        {
            Pedido = pedido;
        }
    }
}
