using AutoMapper;
using ECommerce.DTOs.Login;
using ECommerce.DTOs.Usuarios;
using ECommerce.Exceptions;
using ECommerce.Models;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Logins;
using ECommerce.Services.Usuarios;
using Moq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ECommerce.Tests.Usuarios
{
    public class UsuarioServiceTest
    {
        static string nomeTeste = "Teste";
        static string telefoneTeste = "11999999999";
        static string ruaTeste = "Teste";
        static string cidadeTeste = "Teste";
        static string numeroTeste = "400";
        static string cepTeste = "00000000";
        static string cpfTeste = "00000000000";
        static string emailTeste = "teste@teste.com";
        static string senhaTeste = "senhaTeste";
        private UsuarioCreateDTO CriarUsuarioCreateDTOTeste()
        {
            UsuarioCreateDTO usuario = new UsuarioCreateDTO();

            usuario.Nome = nomeTeste;
            usuario.Telefone = telefoneTeste;
            usuario.Rua = ruaTeste;
            usuario.Cidade = cidadeTeste;
            usuario.NumeroCasa = numeroTeste;
            usuario.Cep = cepTeste;
            usuario.Cpf = cpfTeste;
            usuario.Email = emailTeste;
            usuario.Senha = senhaTeste;

            return usuario;
        }

        [Fact]
        public async Task BuscarUsuario_DeveRetornarUsuario()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(id))
                .ReturnsAsync(usuario);


            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            var resultado = await service.ObterUsuarioPorId(id);

            Assert.NotNull(resultado);
            Assert.Equal(nomeTeste, resultado.Nome);
            Assert.Equal(telefoneTeste, resultado.Telefone);
            Assert.Equal(ruaTeste, resultado.Rua);
            Assert.Equal(cidadeTeste, resultado.Cidade);
            Assert.Equal(numeroTeste, resultado.NumeroCasa);
            Assert.Equal(cepTeste, resultado.Cep);
        }

        [Fact]
        public async Task ProcurarUsuario_DeveLancarErro_QuandoUsuarioNaoExistir()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(It.IsAny<Guid>()))
                .ReturnsAsync((Usuario?)null);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            await Assert.ThrowsAsync<UsuarioNotFound>(() => service.ObterUsuarioPorId(It.IsAny<Guid>()));
        }

        [Fact]
        public async Task DeveCriarUsuario_UsuarioCriadoComSucesso_RepositorioChamadoUmaVez()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };
            var novoUsuario = CriarUsuarioCreateDTOTeste();


            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mockMapper.Setup(x => x.Map<Usuario>(novoUsuario))
                .Returns(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            var usuarioCriado = await service.CriarNovoUsuario(novoUsuario);

            Assert.NotNull(usuarioCriado);
            Assert.NotEqual(Guid.Empty, usuarioCriado.Id);

            Assert.Equal(nomeTeste, usuarioCriado.Nome);
            Assert.Equal(telefoneTeste, usuarioCriado.Telefone);
            Assert.Equal(ruaTeste, usuarioCriado.Rua);
            Assert.Equal(numeroTeste, usuarioCriado.NumeroCasa);
            Assert.Equal(cidadeTeste, usuarioCriado.Cidade);
            Assert.Equal(cepTeste, usuarioCriado.Cep);

            mock.Verify(x => x.CriarUsuario(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task DeveAtualizarUsuario_UsuarioAtualizadoComSucesso()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            string nomeAtualizado = "Novo Nome";
            string novoTelefone = "99111111111";

            UsuarioUpdateDTO usuarioUpdate = new UsuarioUpdateDTO()
            {
                Nome = nomeAtualizado,
                Telefone = novoTelefone
            };

            await service.AtualizarUsuario(id, usuarioUpdate);

            Assert.Equal(nomeAtualizado, usuario.Nome);
            Assert.Equal(novoTelefone, usuario.Telefone);

            mock.Verify(x => x.AtualizarUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task DeveRemoverUsuario_UsuarioDeveLancarExcecao_QuandoBuscado()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            await service.RemoverUsuario(id);

            mock.Verify(x => x.RemoverUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task DeveLancarExcecaoParametroInvalido_QuandoAtualizarTelefone_TelefoneInvalido()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            string novoTelefone = "123";

            UsuarioUpdateDTO usuarioUpdate = new UsuarioUpdateDTO()
            {
                Telefone = novoTelefone
            };

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.AtualizarUsuario(id, usuarioUpdate));
        }

        [Fact]
        public async Task DeveLancarExcecaoParametroInvalido_QuandoAtualizarCep_CepInvalido()
        {
            var id = Guid.NewGuid();

            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste, emailTeste);
            var responseEsperado = new UsuarioResponseDTO()
            {
                Nome = nomeTeste,
                Cep = cepTeste,
                Cidade = cidadeTeste,
                Email = emailTeste,
                Id = id,
                NumeroCasa = numeroTeste,
                Rua = ruaTeste,
                Telefone = telefoneTeste
            };

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();
            var mockMapper = new Mock<IMapper>();

            mockMapper.Setup(x => x.Map<UsuarioResponseDTO>(usuario))
                .Returns(responseEsperado);

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object, mockMapper.Object);

            string novoCep = "123";

            UsuarioUpdateDTO usuarioUpdate = new UsuarioUpdateDTO()
            {
                Cep = novoCep
            };

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.AtualizarUsuario(id, usuarioUpdate));
        }
    }
}
