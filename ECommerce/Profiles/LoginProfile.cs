using AutoMapper;
using ECommerce.DTOs.Login;
using ECommerce.Models;

namespace ECommerce.Profiles
{
    public class LoginProfile : Profile
    {
        public LoginProfile()
        {
            CreateMap<LoginCreateDTO, Login>()
                .ConstructUsing(src => new Login(
                    src.Email,
                    src.Senha,
                    src.IdUsuario
                ));
        }
    }
}
