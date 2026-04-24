using ECommerce.DTOs.Usuarios;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests.Helpers
{
    public class UsuarioHelper
    {
        private readonly HttpClient _client;
        public readonly string _url = "api/Usuarios";

        public UsuarioHelper(HttpClient client)
        {
            _client = client;            
        }
        public UsuarioCreateDTO CriarUsuarioValido()
        {
            return new UsuarioCreateDTO()
            {
                Nome = "Nome Teste",
                Email = $"email_{Guid.NewGuid()}@email.com",
                Cpf = "12121212121",
                Telefone = "11999999999",
                Cep = "00000000",
                Cidade = "SP",
                Rua = "Rua do Teste",
                NumeroCasa = "400",
                Senha = "senhaTeste"
            };
        }
        
        public UsuarioCreateDTO CriarAdminValido()
        {
            return new UsuarioCreateDTO()
            {
                Nome = "Nome Teste",
                Email = "admin@admin.com",
                Cpf = "12121212121",
                Telefone = "11999999999",
                Cep = "00000000",
                Cidade = "SP",
                Rua = "Rua do Teste",
                NumeroCasa = "400",
                Senha = "senhaTeste"
            };
        }
        public UsuarioCreateDTO CriarUsuarioInvalido()
        {
            return new UsuarioCreateDTO()
            {
                Nome = "Nome Teste",
                Email = "email@email.com",
                Cpf = "12121212121",
                Telefone = "116666",
                Cep = "777",
                Cidade = "SP",
                Rua = "Rua do Teste",
                NumeroCasa = "400"
            };
        }
        public async Task<UsuarioResponseDTO> CriarUsuarioValido_NoContexto(UsuarioCreateDTO usuario)
        {
            var postResponse = await _client.PostAsJsonAsync(_url, usuario);
            postResponse.EnsureSuccessStatusCode();

            var usuarioCriado = await postResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();

            if (usuarioCriado == null)
                throw new Exception("Não foi possivel criar o usuario.");

            return usuarioCriado;
        }

        public UsuarioUpdateDTO CriarAtualizacaoValida()
        {
            return new UsuarioUpdateDTO()
            {
                Nome = "Novo Nome Teste",
                NumeroCasa = "300",
                Cep = "01234567",
                Cidade = "Cidade Lá",
                Rua = "Rua Lá",
                Telefone = "11987654321"
            };
        }

        public async Task<HttpResponseMessage> AtualizarUsuario_NoContexto(UsuarioUpdateDTO atualizacao, Guid idUsuario)
        {
            var patchResponse = await _client.PatchAsJsonAsync($"{_url}/{idUsuario}", atualizacao);
            patchResponse.EnsureSuccessStatusCode();

            return patchResponse;
        }

        public async Task<UsuarioResponseDTO> ObterUsuario_NoContexto(Guid idUsuario)
        {
            var getResponse = await _client.GetAsync($"{_url}/{idUsuario}");
            getResponse.EnsureSuccessStatusCode();

            var usuario = await getResponse.Content.ReadFromJsonAsync<UsuarioResponseDTO>();
            if (usuario == null)
                throw new Exception("Não foi possivel buscar o usuário.");

            return usuario;
        }

        public async Task<HttpResponseMessage> DeletarUsuario_NoContexto(Guid idUsuario)
        {
            var deleteResponse = await _client.DeleteAsync($"{_url}/{idUsuario}");
            deleteResponse.EnsureSuccessStatusCode();

            return deleteResponse;
        }
    }
}
