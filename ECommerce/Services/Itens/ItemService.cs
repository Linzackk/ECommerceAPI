using ECommerce.DTOs.Itens;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Itens;

namespace ECommerce.Services.Itens
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        public ItemService(IItemRepository repository)
        {
            _repository = repository;
        }

        private Item CriarItemPorDTO(ItemCreateDTO novoItem)
        {
            return new Item(
                novoItem.Nome,
                novoItem.Descricao,
                novoItem.Estoque,
                novoItem.Preco
            );
        }

        private ItemResponseDTO CriarResponsePorItem(Item item)
        {
            return new ItemResponseDTO()
            {
                Id = item.Id,
                Nome = item.Nome,
                Descricao = item.Descricao,
                Estoque = item.Estoque,
                Preco = item.Preco,
                DataCriacao = item.DataCriacao
            };
        }

        public async Task<Item> ObterPorId(Guid id)
        {
            var item = await _repository.ObterItemPorId(id);
            if (item == null)
                throw new ItemNotFoundException();

            return item;
        }

        public async Task<ItemResponseDTO> CriarNovoItem(ItemCreateDTO novoItem)
        {
            var item = CriarItemPorDTO(novoItem);
            await _repository.CriarItem(item);
            return CriarResponsePorItem(item);
        }

        public async Task<ItemResponseDTO> ObterItemPorId(Guid itemId)
        {
            var item = await ObterPorId(itemId);
            return CriarResponsePorItem(item);
        }

        public async Task AtualizarItem(ItemUpdateDTO itemAtualizado, Guid itemId)
        {
            if (itemAtualizado.EstoqueNovo == null &&
                itemAtualizado.Preco == null
            )    
                throw new ParametroInvalidoException("Nenhuma informação para atualizar foi inserida");

            var item = await ObterPorId(itemId);

            if (itemAtualizado.EstoqueNovo != null)
                item.AumentarEstoque((int)itemAtualizado.EstoqueNovo);

            if (itemAtualizado.Preco != null)
                item.AlterarPreco((decimal)itemAtualizado.Preco);

            await _repository.AtualizarItem(item);
        }

        public async Task RemoverItem(Guid itemId)
        {
            var item = await ObterPorId(itemId);
            await _repository.RemoverItem(item);
        }

        public async Task<IReadOnlyCollection<ItemResponseDTO>> ObterTodos()
        {
            var itens = await _repository.ObterTodos();
            var response = new List<ItemResponseDTO>();

            foreach (var item in itens)
            {
                response.Add(CriarResponsePorItem(item));
            }

            return response;
        }
    }
}
