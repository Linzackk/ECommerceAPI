using ECommerce.Exceptions;

namespace ECommerce.Models
{
    public class Pedido
    {
        public Guid Id { get; private set; }
        public Guid IdUsuario { get; private set; }
        public bool Finalizado { get; private set; }
        public List<PedidoItem> Itens { get; private set; } = new();

        public Pedido(Guid idUsuario)
        {
            if (idUsuario.Equals(Guid.Empty))
                throw new ParametroInvalidoException("ID do usuário não pode estar vazio.");

            Id = Guid.NewGuid();
            IdUsuario = idUsuario;
            Finalizado = false;
        }

        public void AdicionarNovoItem(Item novoItem, int quantidade)
        {
            ValidarPedidoAlteravel();
            var itemExistente = ProcurarPedidoItem(novoItem.Id);

            if (itemExistente != null)
                throw new ParametroInvalidoException("Item já adicionado ao Pedido.");

            var pedidoItem = new PedidoItem(Id, novoItem.Id, quantidade, novoItem.Preco);
            Itens.Add(pedidoItem);
        }

        private PedidoItem? ProcurarPedidoItem(Guid itemId)
        {
            return Itens.FirstOrDefault(pi => pi.IdItem == itemId);
        }

        private void ValidarPedidoAlteravel()
        {
            if (Finalizado)
                throw new ParametroInvalidoException("Não é possivel alterar o pedido, ele já foi fechado.");
        }

        public void AtualizarItem(Guid itemId, int quantidade)
        {
            ValidarPedidoAlteravel();

            var itemExistente = ProcurarPedidoItem(itemId);
            if (itemExistente == null)
                throw new ParametroInvalidoException("Esse item não existe no pedido.");

            if (quantidade <= 0)
                throw new ParametroInvalidoException("Quantidade de Item deve ser maior que 0.");

            itemExistente.DefinirQuantidade(quantidade);
        }
    }
}
