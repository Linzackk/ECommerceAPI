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
        static string senhaHash = BCrypt.Net.BCrypt.HashPassword("senhaTeste");
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
            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste);
            var id = Guid.NewGuid();
            usuario.DefinirLogin(new Login(emailTeste, senhaHash, id));

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mock.Setup(x => x.ObterUsuarioPorId(id))
                .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

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
            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mockLogin
                .Setup(x => x.CriarLogin(It.IsAny<LoginCreateDTO>()))
                .ReturnsAsync(new Login(emailTeste, senhaHash, Guid.NewGuid()));

            mock.Setup(x => x.ObterUsuarioPorId(It.IsAny<Guid>()))
                .ReturnsAsync((Usuario?)null);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

            await Assert.ThrowsAsync<UsuarioNotFound>(() => service.ObterUsuarioPorId(It.IsAny<Guid>()));
        }

        [Fact]
        public async Task DeveCriarUsuario_UsuarioCriadoComSucesso_RepositorioChamadoUmaVez()
        {
            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mockLogin
                .Setup(x => x.CriarLogin(It.IsAny<LoginCreateDTO>()))
                .ReturnsAsync(new Login(emailTeste, senhaHash, Guid.NewGuid()));

            var service = new UsuariosService(mock.Object, mockLogin.Object);

            var usuario = await service.CriarNovoUsuario(CriarUsuarioCreateDTOTeste());

            Assert.NotNull(usuario);
            Assert.NotEqual(Guid.Empty, usuario.Id);

            Assert.Equal(nomeTeste, usuario.Nome);
            Assert.Equal(telefoneTeste, usuario.Telefone);
            Assert.Equal(ruaTeste, usuario.Rua);
            Assert.Equal(numeroTeste, usuario.NumeroCasa);
            Assert.Equal(cidadeTeste, usuario.Cidade);
            Assert.Equal(cepTeste, usuario.Cep);

            mock.Verify(x => x.CriarUsuario(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task DeveAtualizarUsuario_UsuarioAtualizadoComSucesso()
        {
            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste);
            var id = Guid.NewGuid();

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mockLogin
               .Setup(x => x.CriarLogin(It.IsAny<LoginCreateDTO>()))
               .ReturnsAsync(new Login(emailTeste, senhaHash, Guid.NewGuid()));

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

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
            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste);
            usuario.DefinirLogin(new Login(emailTeste, senhaHash, id));

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

            await service.RemoverUsuario(id);

            mock.Verify(x => x.RemoverUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task DeveLancarExcecaoParametroInvalido_QuandoAtualizarTelefone_TelefoneInvalido()
        {
            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste);
            var id = Guid.NewGuid();

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mockLogin
               .Setup(x => x.CriarLogin(It.IsAny<LoginCreateDTO>()))
               .ReturnsAsync(new Login(emailTeste, senhaHash, id));

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

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
            var usuario = new Usuario(nomeTeste, telefoneTeste, ruaTeste, cidadeTeste, numeroTeste, cepTeste, cpfTeste);
            var id = Guid.NewGuid();

            var mock = new Mock<IUsuariosRepository>();
            var mockLogin = new Mock<ILoginService>();

            mockLogin
               .Setup(x => x.CriarLogin(It.IsAny<LoginCreateDTO>()))
               .ReturnsAsync(new Login(emailTeste, senhaHash, id));

            mock.Setup(x => x.ObterUsuarioPorId(id))
               .ReturnsAsync(usuario);

            var service = new UsuariosService(mock.Object, mockLogin.Object);

            string novoCep = "123";

            UsuarioUpdateDTO usuarioUpdate = new UsuarioUpdateDTO()
            {
                Cep = novoCep
            };

            await Assert.ThrowsAsync<ParametroInvalidoException>(() => service.AtualizarUsuario(id, usuarioUpdate));
        }
    }
}
