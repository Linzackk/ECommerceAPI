using ECommerce.Exceptions;
using System.Net.Mail;

namespace ECommerce.Models
{
    public class Login
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Senha { get; private set; }
        public Guid IdUsuario { get; private set; }
        public Usuario Usuario { get; private set; }

        public Login(string email, string senha, Guid idUsuario)
        {
            if (string.IsNullOrEmpty(email))
                throw new ParametroInvalidoException("Email não pode estar vazio.");

            if (!MailAddress.TryCreate(email, out _))
                throw new ParametroInvalidoException("Email inválido.");

            if (string.IsNullOrEmpty(senha))
                throw new ParametroInvalidoException("Senha não pode estar vazia");

            if (Guid.Empty == idUsuario)
                throw new ParametroInvalidoException("ID de usuário não pode estar vazio.");

            Email = email;
            Senha = senha;
            Id = Guid.NewGuid();

        }
    }
}
