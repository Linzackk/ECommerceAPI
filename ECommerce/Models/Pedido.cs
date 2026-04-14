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
            if (Finalizado)
                throw new ParametroInvalidoException("Não é possivel alterar o pedido, ele já foi fechado.");

            var itemExistente = Itens.FirstOrDefault(pi => pi.IdItem == novoItem.Id);

            if (itemExistente != null)
                throw new ParametroInvalidoException("Item já adicionado ao Pedido.");

            var pedidoItem = new PedidoItem(Id, novoItem.Id, quantidade, novoItem.Preco);
            Itens.Add(pedidoItem);
        }
    }
}
