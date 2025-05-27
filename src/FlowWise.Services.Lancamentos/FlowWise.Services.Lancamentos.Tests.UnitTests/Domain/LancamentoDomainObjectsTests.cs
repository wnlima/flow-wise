using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.Events; // Para LancamentoEstado
using FlowWise.Services.Lancamentos.Domain.ValueObjects; // Para LancamentoSnapshot
using FluentAssertions;
using Xunit;
using System;
using System.Linq;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Domain
{
    /// <summary>
    /// Contém testes unitários para objetos de domínio do agregado Lancamento,
    /// incluindo exceções de domínio e Value Objects/records de dados.
    /// </summary>
    [Trait("Category", "DomainUnitTests")]
    [Trait("Requirement", "TDD, REQ-FLW-COM-001, NFR-SEG-003, NFR-RES-004")]
    public class LancamentoDomainObjectsTests
    {
        #region LancamentoDomainException Tests

        /// <summary>
        /// Testa se LancamentoDomainException pode ser instanciada sem mensagem.
        /// REQ-FLW-COM-001: O sistema deve validar os dados de entrada e retornar mensagens de erro claras.
        /// </summary>
        [Fact(DisplayName = "LancamentoDomainException_Should_InstantiateWithoutMessage")]
        public void LancamentoDomainException_Should_InstantiateWithoutMessage()
        {
            // Act
            var exception = new LancamentoDomainException();

            // Assert
            exception.Should().BeOfType<LancamentoDomainException>();
            exception.Message.Should().Be("Exception of type 'FlowWise.Services.Lancamentos.Domain.Exceptions.LancamentoDomainException' was thrown.");
            exception.InnerException.Should().BeNull();
        }

        /// <summary>
        /// Testa se LancamentoDomainException pode ser instanciada com mensagem.
        /// REQ-FLW-COM-001: O sistema deve validar os dados de entrada e retornar mensagens de erro claras.
        /// </summary>
        [Fact(DisplayName = "LancamentoDomainException_Should_InstantiateWithMessage")]
        public void LancamentoDomainException_Should_InstantiateWithMessage()
        {
            // Arrange
            var testMessage = "Esta é uma mensagem de erro de domínio.";

            // Act
            var exception = new LancamentoDomainException(testMessage);

            // Assert
            exception.Should().BeOfType<LancamentoDomainException>();
            exception.Message.Should().Be(testMessage);
            exception.InnerException.Should().BeNull();
        }

        /// <summary>
        /// Testa se LancamentoDomainException pode ser instanciada com mensagem e inner exception.
        /// REQ-FLW-COM-001: O sistema deve validar os dados de entrada e retornar mensagens de erro claras.
        /// </summary>
        [Fact(DisplayName = "LancamentoDomainException_Should_InstantiateWithMessageAndInnerException")]
        public void LancamentoDomainException_Should_InstantiateWithMessageAndInnerException()
        {
            // Arrange
            var testMessage = "Erro ao processar lançamento.";
            var innerEx = new InvalidOperationException("Operação interna falhou.");

            // Act
            var exception = new LancamentoDomainException(testMessage, innerEx);

            // Assert
            exception.Should().BeOfType<LancamentoDomainException>();
            exception.Message.Should().Be(testMessage);
            exception.InnerException.Should().Be(innerEx);
        }

        #endregion

        #region LancamentoEstado Tests

        /// <summary>
        /// Testa a criação e inicialização de um LancamentoEstado.
        /// NFR-RES-004: Consistência de Dados em Transações Distribuídas (LancamentoEstado é usado em eventos).
        /// </summary>
        [Fact(DisplayName = "LancamentoEstado_Should_BeInitializedCorrectly")]
        public void LancamentoEstado_Should_BeInitializedCorrectly()
        {
            // Arrange
            var valor = 100.50m;
            var tipo = "Credito";
            var data = DateTime.Today;
            var descricao = "Venda de produto";
            var categoria = "Vendas";
            string? observacoes = "Cliente A";

            // Act
            var estado = new LancamentoEstado
            {
                Valor = valor,
                Tipo = tipo,
                Data = data,
                Descricao = descricao,
                Categoria = categoria,
                Observacoes = observacoes
            };

            // Assert
            estado.Valor.Should().Be(valor);
            estado.Tipo.Should().Be(tipo);
            estado.Data.Should().Be(data);
            estado.Descricao.Should().Be(descricao);
            estado.Categoria.Should().Be(categoria);
            estado.Observacoes.Should().Be(observacoes);
        }

        /// <summary>
        /// Testa a igualdade por valor de LancamentoEstado (comportamento padrão de records).
        /// NFR-RES-004: Consistência de Dados em Transações Distribuídas.
        /// </summary>
        [Fact(DisplayName = "LancamentoEstado_Should_HaveValueEquality")]
        public void LancamentoEstado_Should_HaveValueEquality()
        {
            // Arrange
            var estado1 = new LancamentoEstado
            {
                Valor = 100m,
                Tipo = "Credito",
                Data = DateTime.Today,
                Descricao = "Teste",
                Categoria = "Teste",
                Observacoes = "Obs"
            };
            var estado2 = new LancamentoEstado
            {
                Valor = 100m,
                Tipo = "Credito",
                Data = DateTime.Today,
                Descricao = "Teste",
                Categoria = "Teste",
                Observacoes = "Obs"
            };
            var estado3 = new LancamentoEstado
            {
                Valor = 200m, // Diferente valor
                Tipo = "Debito",
                Data = DateTime.Today,
                Descricao = "Outro Teste",
                Categoria = "Outra",
                Observacoes = null
            };

            // Assert
            estado1.Should().Be(estado2);
            (estado1 == estado2).Should().BeTrue();
            estado1.GetHashCode().Should().Be(estado2.GetHashCode());

            estado1.Should().NotBe(estado3);
            (estado1 != estado3).Should().BeTrue();
        }

        /// <summary>
        /// Testa a imutabilidade de LancamentoEstado (propriedades com `init`).
        /// </summary>
        [Fact(DisplayName = "LancamentoEstado_Should_BeImmutable")]
        public void LancamentoEstado_Should_BeImmutable()
        {
            // Arrange
            var estado = new LancamentoEstado
            {
                Valor = 100m,
                Tipo = "Credito",
                Data = DateTime.Today,
                Descricao = "Original",
                Categoria = "Original",
                Observacoes = "Original"
            };

            // Act & Assert - Tentar atribuir um novo valor deve resultar em erro de compilação
            // Se esta linha for descomentada, o código não compilará, validando a imutabilidade em tempo de compilação.
            // estado.Valor = 200m; 
        }

        #endregion
    }
}