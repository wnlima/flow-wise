using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FluentAssertions;

namespace FlowWise.Services.Consolidacao.Tests.UnitTests.Domain.Entities
{
    /// <summary>
    /// Contém testes unitários para a entidade <see cref="SaldoDiario"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes cobrem a lógica de negócio principal da entidade SaldoDiario,
    /// incluindo sua criação, aplicação e reversão de lançamentos.
    /// REQ-FLW-CON-001: Consolidar lançamentos diários para gerar saldo.
    /// RN-CON-001: Saldo Diário deve ser calculado com base em créditos e débitos.
    /// RN-CON-002: Não é possível criar saldo diário para data futura.
    /// HU-006: Como um usuário, eu quero ver o saldo total de um dia específico.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class SaldoDiarioTests
    {
        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.Create(DateOnly)"/> cria uma instância válida
        /// de <see cref="SaldoDiario"/> com valores iniciais corretos.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante que um SaldoDiario pode ser inicializado corretamente.
        /// TDD: Cenário de sucesso na criação de SaldoDiario.
        /// </remarks>
        [Fact]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001")]
        public void Create_ValidDate_ReturnsSaldoDiarioWithCorrectInitialValues()
        {
            // Arrange
            var data = DateTime.Today;

            // Act
            var saldoDiario = SaldoDiario.Create(data);

            // Assert
            saldoDiario.Should().NotBeNull();
            saldoDiario.Data.Should().Be(data);
            saldoDiario.TotalCreditos.Should().Be(0);
            saldoDiario.TotalDebitos.Should().Be(0);
            saldoDiario.SaldoTotal.Should().Be(0);
            saldoDiario.UltimaAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.Create(DateOnly)"/> lança <see cref="ConsolidacaoDomainException"/>
        /// quando a data fornecida é futura.
        /// </summary>
        /// <remarks>
        /// RN-CON-002: Não é possível criar saldo diário para data futura.
        /// REQ-FLW-CON-001: Validação de regras de negócio na criação de SaldoDiario.
        /// TDD: Cenário de falha na criação de SaldoDiario (data futura).
        /// </remarks>
        [Fact]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, RN-CON-002")]
        public void Create_FutureDate_ThrowsConsolidacaoDomainException()
        {
            // Arrange
            var dataFutura = DateTime.Today.AddDays(1);

            // Act
            Action act = () => SaldoDiario.Create(dataFutura);

            // Assert
            act.Should().Throw<ConsolidacaoDomainException>()
               .WithMessage("A data para consolidação não pode ser futura.");
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.AplicarLancamento(decimal, TipoLancamento)"/>
        /// atualiza corretamente o saldo para um lançamento de crédito.
        /// </summary>
        /// <param name="valorLancamento">O valor do lançamento a ser aplicado.</param>
        /// <remarks>
        /// REQ-FLW-CON-001: Cálculo de saldo para créditos.
        /// RN-CON-001: Saldo Diário deve ser calculado com base em créditos.
        /// TDD: Cenário de sucesso na aplicação de lançamento de crédito.
        /// </remarks>
        [Theory]
        [InlineData(100.00)]
        [InlineData(50.50)]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, RN-CON-001")]
        public void AplicarLancamento_Credito_UpdatesSaldoCorrectly(decimal valorLancamento)
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            var saldoInicial = saldoDiario.SaldoTotal;
            var totalCreditosInicial = saldoDiario.TotalCreditos;

            // Act
            saldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), valorLancamento);

            // Assert
            saldoDiario.TotalCreditos.Should().Be(totalCreditosInicial + valorLancamento);
            saldoDiario.SaldoTotal.Should().Be(saldoInicial + valorLancamento);
            saldoDiario.UltimaAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.AplicarLancamento(decimal, TipoLancamento)"/>
        /// atualiza corretamente o saldo para um lançamento de débito.
        /// </summary>
        /// <param name="valorLancamento">O valor do lançamento a ser aplicado.</param>
        /// <remarks>
        /// REQ-FLW-CON-001: Cálculo de saldo para débitos.
        /// RN-CON-001: Saldo Diário deve ser calculado com base em débitos.
        /// TDD: Cenário de sucesso na aplicação de lançamento de débito.
        /// </remarks>
        [Theory]
        [InlineData(75.00)]
        [InlineData(25.75)]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, RN-CON-001")]
        public void AplicarLancamento_Debito_UpdatesSaldoCorrectly(decimal valorLancamento)
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            var saldoInicial = saldoDiario.SaldoTotal;
            var totalDebitosInicial = saldoDiario.TotalDebitos;

            // Act
            saldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), valorLancamento);

            // Assert
            saldoDiario.TotalDebitos.Should().Be(totalDebitosInicial + valorLancamento);
            saldoDiario.SaldoTotal.Should().Be(saldoInicial - valorLancamento);
            saldoDiario.UltimaAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.AplicarLancamento(decimal, TipoLancamento)"/>
        /// lança <see cref="ConsolidacaoDomainException"/> para um tipo de lançamento inválido.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Tratamento de erro para tipos de lançamento desconhecidos.
        /// TDD: Cenário de falha na aplicação de lançamento (tipo inválido).
        /// </remarks>
        [Fact]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001")]
        public void AplicarLancamento_InvalidTipoLancamento_ThrowsConsolidacaoDomainException()
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            var valor = 100m;
            var tipoInvalido = "TIPO_INVALIDO";

            // Act
            Action act = () => saldoDiario.AplicarLancamento(tipoInvalido, valor);

            // Assert
            act.Should().Throw<ConsolidacaoDomainException>()
               .WithMessage($"Tipo de lançamento inválido para aplicação de saldo.");
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.ReverterLancamento(decimal, TipoLancamento)"/>
        /// atualiza corretamente o saldo para a reversão de um lançamento de crédito.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Reversão de saldo para créditos.
        /// RN-CON-001: Saldo Diário deve ser ajustado na reversão.
        /// TDD: Cenário de sucesso na reversão de lançamento de crédito.
        /// </remarks>
        [Theory]
        [InlineData(100.00)]
        [InlineData(50.50)]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, RN-CON-001")]
        public void ReverterLancamento_Credito_UpdatesSaldoCorrectly(decimal valorLancamento)
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            saldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), valorLancamento);
            var saldoPreReversao = saldoDiario.SaldoTotal;
            var totalCreditosPreReversao = saldoDiario.TotalCreditos;

            // Act
            saldoDiario.ReverterLancamento(TipoLancamento.Credito.ToString(), valorLancamento);

            // Assert
            saldoDiario.TotalCreditos.Should().Be(totalCreditosPreReversao - valorLancamento);
            saldoDiario.SaldoTotal.Should().Be(saldoPreReversao - valorLancamento);
            saldoDiario.UltimaAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.ReverterLancamento(decimal, TipoLancamento)"/>
        /// atualiza corretamente o saldo para a reversão de um lançamento de débito.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Reversão de saldo para débitos.
        /// RN-CON-001: Saldo Diário deve ser ajustado na reversão.
        /// TDD: Cenário de sucesso na reversão de lançamento de débito.
        /// </remarks>
        [Theory]
        [InlineData(75.00)]
        [InlineData(25.75)]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, RN-CON-001")]
        public void ReverterLancamento_Debito_UpdatesSaldoCorrectly(decimal valorLancamento)
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            saldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), valorLancamento); // Aplica primeiro
            var saldoPreReversao = saldoDiario.SaldoTotal;
            var totalDebitosPreReversao = saldoDiario.TotalDebitos;

            // Act
            saldoDiario.ReverterLancamento(TipoLancamento.Debito.ToString(), valorLancamento);

            // Assert
            saldoDiario.TotalDebitos.Should().Be(totalDebitosPreReversao - valorLancamento);
            saldoDiario.SaldoTotal.Should().Be(saldoPreReversao + valorLancamento);
            saldoDiario.UltimaAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Testa se o método <see cref="SaldoDiario.ReverterLancamento(decimal, TipoLancamento)"/>
        /// lança <see cref="ConsolidacaoDomainException"/> para um tipo de lançamento inválido.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Tratamento de erro para tipos de lançamento desconhecidos na reversão.
        /// TDD: Cenário de falha na reversão de lançamento (tipo inválido).
        /// </remarks>
        [Fact]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001")]
        public void ReverterLancamento_InvalidTipoLancamento_ThrowsConsolidacaoDomainException()
        {
            // Arrange
            var saldoDiario = SaldoDiario.Create(DateTime.Today);
            var valor = 100m;
            var tipoInvalido = "TipoInvalido";// Um valor de enum inválido

            // Act
            Action act = () => saldoDiario.ReverterLancamento(tipoInvalido, valor);

            // Assert
            act.Should().Throw<ConsolidacaoDomainException>()
               .WithMessage($"Tipo de lançamento inválido para reversão de saldo.");
        }
    }
}