using AutoMapper;
using ECommerce.DTOs.Usuarios;
using ECommerce.Models;

namespace ECommerce.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioResponseDTO>()
                .ForMember(dest => dest.NumeroCasa, opt => opt.MapFrom(src => src.Numero));
            CreateMap<UsuarioCreateDTO, Usuario>()
                .ConstructUsing(src => new Usuario(
                    src.Nome,
                    src.Telefone,
                    src.Rua,
                    src.Cidade,
                    src.NumeroCasa,
                    src.Cep,
                    src.Cpf,
                    src.Email
                ));
        }
    }
}
