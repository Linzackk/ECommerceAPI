using AutoMapper;
using ECommerce.DTOs.Itens;
using ECommerce.Models;

namespace ECommerce.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile() 
        {
            CreateMap<Item, ItemResponseDTO>();
            CreateMap<ItemCreateDTO, Item>()
                .ConstructUsing(src => new Item(
                    src.Nome,
                    src.Descricao,
                    src.Estoque,
                    src.Preco
                ));
        }
    }
}
