using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Usuarios
{
    public class UsuarioEndpointsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly string _url = "api/Usuarios";
        private readonly HttpClient _client;

        public UsuarioEndpointsTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Deve_ProcurarUsuarioInexistente_Retornar404()
        {
            var id = Guid.NewGuid().ToString();
            var response = await _client.GetAsync($"{_url}/{id}");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
