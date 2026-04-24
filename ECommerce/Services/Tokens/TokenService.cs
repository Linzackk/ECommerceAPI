using ECommerce.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Services.Tokens
{
    public class TokenService : ITokenService
    {
        private string _key;
        private List<string> _admin;

        public TokenService(IConfiguration config)
        {
            _key = config["Jwt:Key"];
            _admin = config.GetSection("AdminEmail").Get<List<String>>();
        }
        public string GerarToken(Login login)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            var isAdmin = _admin.Contains(login.Email);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.IdUsuario.ToString()),
                new Claim("isAdmin", isAdmin.ToString().ToLower())
            };

            var credential = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = credential
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
