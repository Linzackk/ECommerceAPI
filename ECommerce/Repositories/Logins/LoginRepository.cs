using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories.Logins
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbContext _context;

        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyCollection<Login>> ObterTodos()
        {
            return await _context.Login.ToListAsync();
        }

        public async Task CriarLogin(Login novoLogin)
        {
            Console.WriteLine("CRIANDO LOGIN");
            Console.WriteLine($"{novoLogin.IdUsuario} - {novoLogin.Email}  - {novoLogin.Senha}");
            await _context.Login.AddAsync(novoLogin);
            await _context.SaveChangesAsync();
        }

        public async Task<Login?> ObterPorEmail(string email)
        {
            var login = await _context.Login.FirstOrDefaultAsync(login => login.Email == email);
            return login;
        }

        public async Task RemoverLogin(Login login)
        {
            _context.Login.Remove(login);
            await _context.SaveChangesAsync();
        }

        public async Task<Login?> ProcurarLoginPorUsuarioId(Guid usuarioId)
        {
            var login = await _context.Login.FirstOrDefaultAsync(l => l.IdUsuario == usuarioId);
            return login;
        }
    }
}
