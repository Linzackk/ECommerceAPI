using ECommerce.Exceptions;

namespace ECommerce.Models
{
    public class Item
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public int Estoque { get; private set; }
        public decimal Preco { get; private set; }
        public DateOnly DataCriacao { get; private set; }

        public Item(string nome, string descricao, int estoque, decimal preco)
        {
            if (string.IsNullOrEmpty(nome))
                throw new ParametroInvalidoException("Nome do Item não pode ser nulo");

            if (string.IsNullOrEmpty(descricao))
                throw new ParametroInvalidoException("Descrição do Item não pode ser nulo");

            if (estoque <= 0)
                throw new ParametroInvalidoException("Estoque deve ser maior que 0.");

            if (preco <= 0)
                throw new ParametroInvalidoException("Preço deve ser maior que 0.");

            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            Estoque = estoque;
            Preco = preco;
            DataCriacao = DateOnly.FromDateTime(DateTime.Now);
        }

        public void AlterarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0)
                throw new ParametroInvalidoException("Preço deve ser maior que 0.");

            Preco = novoPreco;
        }

        public void AumentarEstoque(int valorAumentado)
        {
            if (valorAumentado <= 0)
                throw new ParametroInvalidoException("Apenas valores a cima de 0.");

            Estoque += valorAumentado;
        }

        public void ReduzirEstoque(int valorReduzido)
        {
            if (valorReduzido <= 0)
                throw new ParametroInvalidoException("Apenas valores a cima de 0.");

            if (Estoque < valorReduzido)
                throw new EstoqueInsuficienteException(Nome);

            Estoque -= valorReduzido;
        }
    }
}
