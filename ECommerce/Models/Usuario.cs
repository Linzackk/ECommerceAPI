using ECommerce.Exceptions;
using Microsoft.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;

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
                throw new ParametroInvalidoException("Nome não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(telefone))
                throw new ParametroInvalidoException("Telefone não pode ser nulo ou vazio.");

            var telefoneNormalizado = NormalizarTelefone(telefone);
            if (telefoneNormalizado.Length != 11 || !(int.TryParse(telefoneNormalizado, out _)))
                throw new ParametroInvalidoException("Número de Telefone inválido.");

            if (string.IsNullOrEmpty(rua))
                throw new ParametroInvalidoException("Rua não pode ser nula ou vazio.");

            if (string.IsNullOrEmpty(cidade))
                throw new ParametroInvalidoException("Cidade não pode ser nula ou vazio.");

            if (string.IsNullOrEmpty(numero))
                throw new ParametroInvalidoException("Numero não pode ser nulo ou vazio.");

            var cepNormalizado = NormalizarCep(cep);
            if (cepNormalizado.Length != 8 || !(int.TryParse(cepNormalizado, out _)))
                throw new ParametroInvalidoException("CEP Inválido.");

            if (string.IsNullOrEmpty(cep))
                throw new ParametroInvalidoException("CEP não pode ser nulo ou vazio.");

            if (string.IsNullOrEmpty(cpf))
                throw new ParametroInvalidoException("CPF não pode ser nulo ou vazio.");

            Id = Guid.NewGuid();
            Nome = nome;
            Telefone = telefone;
            Rua = rua;
            Cidade = cidade;
            Numero = numero;
            Cep = cep;
            CPF = cpf;
        }

        public void AtualizarNome(string novoNome)
        {
            if (string.IsNullOrEmpty(novoNome))
                throw new ParametroInvalidoException("Nome não pode ser nulo ou vazio.");

            Nome = novoNome;
        }

        public void AtualizarTelefone(string novoTelefone)
        {
            var telefoneNormalizado = NormalizarTelefone(novoTelefone);
            if (telefoneNormalizado.Length != 11 || !(int.TryParse(novoTelefone, out _)))
                throw new ParametroInvalidoException("Número de Telefone inválido.");

            Telefone = novoTelefone;
        }

        private string NormalizarTelefone(string telefone)
        {
            telefone = telefone.Replace('(', '\0');
            telefone = telefone.Replace(')', '\0');
            telefone = telefone.Replace('-', '\0');
            telefone = telefone.Replace(' ', '\0');
            return telefone;
        }

        public void AtualizarRua(string novaRua)
        {
            if (string.IsNullOrEmpty(novaRua))
                throw new ParametroInvalidoException("Rua não pode ser nula ou vazio.");

            Rua = novaRua;
        }

        public void AtualizarCidade(string novaCidade)
        {
            if (string.IsNullOrEmpty(novaCidade))
                throw new ParametroInvalidoException("Cidade não pode ser nula ou vazio.");

            Cidade = novaCidade;
        }

        public void AtualizarNumero(string novoNumero)
        {
            if (string.IsNullOrEmpty(novoNumero))
                throw new ParametroInvalidoException("Número  da Casa não pode ser nulo ou vazio.");

            Numero = novoNumero;
        }

        public void AtualizarCep(string novoCep)
        {
            if (string.IsNullOrEmpty(novoCep))
                throw new ParametroInvalidoException("CEP não pode ser nulo ou vazio.");

            var cepNormalizado = NormalizarCep(novoCep);
            if (cepNormalizado.Length != 8 || !(int.TryParse(cepNormalizado, out _)))
                throw new ParametroInvalidoException("CEP Inválido.");

            Cep = novoCep;
        }

        public string NormalizarCep(string cep)
        {
            cep = cep.Replace(' ', '\0');
            cep = cep.Replace('-', '\0');
            return cep;
        }
    }
}
