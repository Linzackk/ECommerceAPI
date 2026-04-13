using ECommerce.Models;

namespace ECommerce.Repositories.Itens
{
    public interface IItemRepository
    {
        Task CriarItem(Item novoItem);
        Task<Item?> ObterItemPorId(Guid id);
        Task AtualizarItem(Item itemAtualizado);

        Task RemoverItem(Item item);
        Task<IReadOnlyCollection<Item>> ObterTodos();
    }
}
