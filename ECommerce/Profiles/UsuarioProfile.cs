using AutoMapper;
using ECommerce.DTOs.Usuarios;
using ECommerce.Models;

namespace ECommerce.Profiles
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioResponseDTO>();
            CreateMap<UsuarioCreateDTO, Usuario>();
        }
    }
}
