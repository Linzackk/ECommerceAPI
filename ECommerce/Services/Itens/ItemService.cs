using AutoMapper;
using ECommerce.DTOs.Itens;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Itens;

namespace ECommerce.Services.Itens
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;
        private readonly IMapper _mapper;
        public ItemService(IItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            var item = _mapper.Map<Item>(novoItem);
            await _repository.CriarItem(item);
            return _mapper.Map<ItemResponseDTO>(item);
        }

        public async Task<ItemResponseDTO> ObterItemPorId(Guid itemId)
        {
            var item = await ObterPorId(itemId);
            return _mapper.Map<ItemResponseDTO>(item);
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

            return _mapper.Map<IReadOnlyCollection<ItemResponseDTO>>(itens);
        }
    }
}
