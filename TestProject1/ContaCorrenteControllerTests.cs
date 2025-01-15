using Xunit;
using NSubstitute;
using Questao5.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Questao5.Domain.Entities;
using Questao5.Domain.Interfaces;
using Questao5.Application.Commands.Requests;
using System;
using Questao5.Domain.Entities.Questao5.Domain;

namespace Questao5.Tests.Controllers
{ 
    public class ContaCorrenteControllerTests
    {
        private readonly IDbConnectionWrapper _dbConnectionWrapper;
        private readonly ContaCorrenteController _controller;

        public ContaCorrenteControllerTests()
        {
            _dbConnectionWrapper = Substitute.For<IDbConnectionWrapper>();
            _controller = new ContaCorrenteController(_dbConnectionWrapper);
        }

        [Fact]
        public async Task ConsultarSaldo_ContaValida_DeveRetornarOk()
        {
            // Arrange
            var idContaCorrente = "123";
            var conta = new ContaCorrente { IdContaCorrente = "123", Ativo = 1, Numero = "12345", Nome = "João" };

            // Configurando o mock para QueryFirstOrDefaultAsync<ContaCorrente>
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Configurando o mock para QueryFirstOrDefaultAsync<decimal>
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<decimal>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(500m));

            // Act
            var result = await _controller.ConsultarSaldo(idContaCorrente);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            // Convertendo o objeto de resposta para um tipo anônimo
            var expectedResponse = new { Numero = "12345", Nome = "João", Saldo = 500m };
            var actualResponse = new
            {
                Numero = (string)response.GetType().GetProperty("Numero").GetValue(response, null),
                Nome = (string)response.GetType().GetProperty("Nome").GetValue(response, null),
                Saldo = (decimal)response.GetType().GetProperty("Saldo").GetValue(response, null)
            };

            Assert.Equal(expectedResponse.Numero, actualResponse.Numero);
            Assert.Equal(expectedResponse.Nome, actualResponse.Nome);
            Assert.Equal(expectedResponse.Saldo, actualResponse.Saldo);
        }

        [Fact]
        public async Task ConsultarSaldo_ContaNaoCadastrada_DeveRetornarBadRequest()
        {
            // Arrange
            var idContaCorrente = "123";

            // Configurando o mock para retornar null
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<ContaCorrente>(null));

            // Act
            var result = await _controller.ConsultarSaldo(idContaCorrente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Conta não cadastrada", response.Mensagem);
            Assert.Equal("INVALID_ACCOUNT", response.Tipo);
        }

        [Fact]
         public async Task ConsultarSaldo_ContaInativa_DeveRetornarBadRequest()
        {
            // Arrange
            var idContaCorrente = "123";
            var conta = new ContaCorrente { IdContaCorrente = "123", Ativo = 0, Numero = "12345", Nome = "João" };

            // Configurando o mock para QueryFirstOrDefaultAsync<ContaCorrente>
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Act
            var result = await _controller.ConsultarSaldo(idContaCorrente);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Conta inativa", response.Mensagem);
            Assert.Equal("INACTIVE_ACCOUNT", response.Tipo);
        }

        [Fact]
        public async Task MovimentarConta_ValorInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdContaCorrente = "123", TipoMovimento = "c", Valor = -100 };

            // Act
            var result = await _controller.MovimentarConta(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Valor inválido", response.Mensagem);
            Assert.Equal("INVALID_VALUE", response.Tipo);
        }

        [Fact]
        public async Task MovimentarConta_TipoMovimentoInvalido_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdContaCorrente = "123", TipoMovimento = "x", Valor = 100 };

            // Act
            var result = await _controller.MovimentarConta(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Tipo de movimento inválido", response.Mensagem);
            Assert.Equal("INVALID_TYPE", response.Tipo);
        }

        [Fact]
        public async Task MovimentarConta_ContaNaoCadastrada_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdContaCorrente = "123", TipoMovimento = "c", Valor = 100 };

            // Configurando o mock para retornar null
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult<ContaCorrente>(null));

            // Act
            var result = await _controller.MovimentarConta(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Conta não cadastrada", response.Mensagem);
            Assert.Equal("INVALID_ACCOUNT", response.Tipo);
        }

        [Fact]
        public async Task MovimentarConta_ContaInativa_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdContaCorrente = "123", TipoMovimento = "c", Valor = 100 };
            var conta = new ContaCorrente { IdContaCorrente = "123", Ativo = 0, Numero = "12345", Nome = "João" };

            // Configurando o mock para QueryFirstOrDefaultAsync<ContaCorrente>
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Act
            var result = await _controller.MovimentarConta(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = badRequestResult.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("Conta inativa", response.Mensagem);
            Assert.Equal("INACTIVE_ACCOUNT", response.Tipo);
        }

        [Fact]
        public async Task MovimentarConta_ContaValida_DeveRetornarOk()
        {
            // Arrange
            var request = new MovimentacaoRequest { IdContaCorrente = "123", TipoMovimento = "c", Valor = 100 };
            var conta = new ContaCorrente { IdContaCorrente = "123", Ativo = 1, Numero = "12345", Nome = "João" };

            // Configurando o mock para QueryFirstOrDefaultAsync<ContaCorrente>
            _dbConnectionWrapper.QueryFirstOrDefaultAsync<ContaCorrente>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(conta));

            // Configurando o mock para ExecuteAsync
            _dbConnectionWrapper.ExecuteAsync(Arg.Any<string>(), Arg.Any<object>())
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.MovimentarConta(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);
            try
            {
                Assert.NotNull(response);

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
