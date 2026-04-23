using ECommerce.Models;

namespace ECommerce.Services.Tokens
{
    public interface ITokenService
    {
        string GerarToken(Usuario usuario);
    }
}
