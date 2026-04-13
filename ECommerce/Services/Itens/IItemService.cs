using ECommerce.DTOs.Itens;
using ECommerce.Models;

namespace ECommerce.Services.Itens
{
    public interface IItemService
    {
        public Task<ItemResponseDTO> CriarNovoItem(ItemCreateDTO novoItem);
        public Task<ItemResponseDTO> ObterItemPorId(Guid itemId);
        public Task AtualizarItem(ItemUpdateDTO itemAtualizado, Guid itemId);
        public Task RemoverItem(Guid itemId);
        public Task<IReadOnlyCollection<ItemResponseDTO>> ObterTodos();
    }
}
