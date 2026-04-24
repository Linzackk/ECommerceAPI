using ECommerce.Models;

namespace ECommerce.Services.Tokens
{
    public interface ITokenService
    {
        string GerarToken(Login login);
    }
}
