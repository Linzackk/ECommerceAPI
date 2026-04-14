using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories.Pedidos
{
    public class PedidosRepository : IPedidosRepository
    {
        private readonly AppDbContext _context;
        public PedidosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarNovoPedido(Pedido novoPedido)
        {
            await _context.Pedidos.AddAsync(novoPedido);
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido?> ObterPedidoPorId(Guid pedidoId)
        {
            var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id.Equals(pedidoId));
            return pedido;
        }

        public async Task<IReadOnlyCollection<Pedido>> ObterPedidosPorIdUsuario(Guid idUsuario)
        {
            var pedidos = await _context.Pedidos.Where(p => p.IdUsuario.Equals(idUsuario)).ToListAsync();
            return pedidos;
        }

        public async Task RemoverPedido(Pedido pedido)
        {
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarPedido(Pedido pedido)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido?> ObterPedidoAberto(Guid idUsuario)
        {
            var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.IdUsuario.Equals(idUsuario) && p.Finalizado == false);
            return pedido;
        }
    }
}
