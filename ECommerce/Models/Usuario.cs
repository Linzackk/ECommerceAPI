namespace ECommerce.Models
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Telefone { get; private set; }
        public string Rua { get; private set; }
        public string Cidade { get; private set; }
        public string Numero { get; private set; }
        public string Cep { get; private set; }
        public string CPF { get; private set; }
        public Login Login { get; private set; }
        public List<Pedido> Pedidos { get; private set; } = new();

        public Usuario(string nome, string telefone, string rua, string cidade, string numero, string cep, string cpf)
        {
            if (string.IsNullOrEmpty(nome))
                throw new ArgumentException("Nome não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(telefone))
                throw new ArgumentException("Telefone não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(rua))
                throw new ArgumentException("Rua não pode ser nula ou vazio.");

            if (string.IsNullOrEmpty(cidade))
                throw new ArgumentException("Cidade não pode ser nula ou vazio.");

            if (string.IsNullOrEmpty(numero))
                throw new ArgumentException("Numero não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(cep))
                throw new ArgumentException("CEP não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(cpf))
                throw new ArgumentException("CPF não pode ser nulo ou vazio.");

            Id = Guid.NewGuid();
            Nome = nome;
            Telefone = telefone;
            Rua = rua;
            Cidade = cidade;
            Numero = numero;
            Cep = cep;
            CPF = cpf;
        }
    }
}
