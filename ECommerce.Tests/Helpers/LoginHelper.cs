using ECommerce.DTOs.Login;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Helpers
{
    public class LoginHelper
    {
        private readonly HttpClient _client;
        private readonly string _url = "api/Login";

        public LoginHelper(HttpClient client)
        {
            _client = client;
        }
        public LoginEntrarDTO CriarLoginEntrarValido(string email, string senha)
        {
            return new LoginEntrarDTO()
            {
                Email = email,
                Senha = senha
            };
        }
        public LoginEntrarDTO CriarLoginEntrarInvalido()
        {
            return new LoginEntrarDTO()
            {
                Email = "qualquerCoisaAi@Invalido.com",
                Senha = "Senha Incorreta"
            };
        }
        public async Task<string> FazerLoginCorretamente(LoginEntrarDTO credenciaisValidas)
        {
            var loginPostResponse = await _client.PostAsJsonAsync(_url, credenciaisValidas);
            if (loginPostResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception("Login não foi feito corretamente, verifique se as informações estão corretas.");

            var response = await loginPostResponse.Content.ReadAsStringAsync();
            if (response == null)
                throw new Exception("Nenhuma resposta foi enviada.");

            return response;
        }

        public void AdicionarTokenAoClient(string token)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
