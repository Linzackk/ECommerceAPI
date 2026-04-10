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

        public async Task CriarLogin(Login novoLogin)
        {
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
    }
}
