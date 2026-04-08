using ECommerce.Models;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Usuarios;
using Moq;

namespace ECommerce.Tests.Usuarios
{
    public class UsuarioServiceTest
    {
        [Fact]
        public async void BuscarUsuario_DeveRetornarUsuario()
        {
            var mock = new Mock<IUsuariosRepository>();

            string nome = "Teste";
            string telefone = "11999999999";
            string rua = "Teste";
            string cidade = "Teste";
            string numero = "400";
            string cep = "00000000";
            string cpf = "00000000000";

            mock.Setup(x => x.ObterUsuarioPorId(It.IsAny<Guid>()))
                .ReturnsAsync(new Usuario(nome, telefone, rua, cidade, numero, cep, cpf));

            var service = new UsuariosService(mock.Object);

            var resultado = await service.ObterUsuarioPorId(It.IsAny<Guid>());

            Assert.NotNull(resultado);
            Assert.Equal(nome, resultado.Nome);
            Assert.Equal(telefone, resultado.Telefone);
            Assert.Equal(rua, resultado.Rua);
            Assert.Equal(cidade, resultado.Cidade);
            Assert.Equal(numero, resultado.NumeroCasa);
            Assert.Equal(cep, resultado.Cep);
        }
    }
}
