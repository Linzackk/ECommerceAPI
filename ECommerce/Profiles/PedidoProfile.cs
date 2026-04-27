using AutoMapper;
using ECommerce.DTOs.Pedidos;
using ECommerce.Models;

namespace ECommerce.Profiles
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<PedidoItem, PedidoItemResponseDTO>()
                .ForMember(dest => dest.IdItem, opt => opt.MapFrom(src => src.IdItem))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Item != null ? src.Item.Nome : string.Empty))
                .ForMember(dest => dest.Preco, opt => opt.MapFrom(src => src.ValorUnitario))
                .ForMember(dest => dest.Quantidade, opt => opt.MapFrom(src => src.Quantidade));

            CreateMap<Pedido, PedidoResponseDTO>()
                .ForMember(dest => dest.Itens, opts => opts.MapFrom(src => src.Itens));
        }
    }
}
