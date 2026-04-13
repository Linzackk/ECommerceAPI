namespace ECommerce.Exceptions
{
    public class EstoqueInsuficienteException : Exception
    {
        public EstoqueInsuficienteException(string produto)
            : base($"Estoque Insuficiente do produto {produto}") { }
    }
}
