using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories.Usuarios
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly AppDbContext _context;

        public UsuariosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarUsuario(Usuario novoUsuario)
        {
            Console.WriteLine("CRIANDO NOVO USUARIO");
            Console.WriteLine($"{novoUsuario.Id} - {novoUsuario.Email}");
            await _context.Usuarios.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObterUsuarioPorId(Guid usuarioId)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(user => user.Id == usuarioId);
            return usuario;
        }

        public async Task RemoverUsuario(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarUsuario(Usuario usuarioAtualizado)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Usuario>> ObterTodos()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return usuarios;
        }
    }
}
