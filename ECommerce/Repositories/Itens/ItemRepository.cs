using ECommerce.Data;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories.Itens
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CriarItem(Item novoItem)
        {
            await _context.Itens.AddAsync(novoItem);
            await _context.SaveChangesAsync();
        }

        public async Task<Item?> ObterItemPorId(Guid itemId)
        {
            var item = await _context.Itens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == itemId);
            return item;
        }

        public async Task  AtualizarItem(Item itemAtualizado)
        {
            await _context.SaveChangesAsync();
        }

        public async Task RemoverItem(Item item)
        {
            _context.Itens.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<Item>> ObterTodos()
        {
            var itens = await _context.Itens.ToListAsync();
            return itens;
        }
    }
}
