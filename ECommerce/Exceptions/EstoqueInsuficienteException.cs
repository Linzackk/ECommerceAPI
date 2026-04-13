namespace ECommerce.Exceptions
{
    public class EstoqueInsuficienteException : BadRequestException
    {
        public EstoqueInsuficienteException(string produto)
            : base($"Estoque Insuficiente do produto {produto}") { }
    }
}
