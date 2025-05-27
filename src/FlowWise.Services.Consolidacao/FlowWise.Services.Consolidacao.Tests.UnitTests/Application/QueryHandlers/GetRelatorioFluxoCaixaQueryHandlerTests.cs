using FlowWise.Services.Consolidacao.Application.QueryHandlers;
using FlowWise.Services.Consolidacao.Application.Queries;
using FlowWise.Services.Consolidacao.Application.Dtos;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using NSubstitute.ExceptionExtensions;

namespace FlowWise.Services.Consolidacao.Tests.UnitTests.Application.QueryHandlers
{
    /// <summary>
    /// Contém testes unitários para o <see cref="GetRelatorioFluxoCaixaQueryHandler"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes verificam se o handler de consulta para o relatório de fluxo de caixa
    /// calcula corretamente os totais e saldos para um período específico.
    /// REQ-FLW-CON-002: Gerar relatório de fluxo de caixa por período.
    /// HU-007: Como um usuário, eu quero gerar um relatório de fluxo de caixa para um período.
    /// NFR-PERF-004: O cálculo do relatório de fluxo de caixa deve ser otimizado para períodos de até 30 dias.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class GetRelatorioFluxoCaixaQueryHandlerTests
    {
        private readonly ISaldoDiarioRepository _mockSaldoDiarioRepository;
        private readonly ILogger<GetRelatorioFluxoCaixaQueryHandler> _mockLogger;
        private readonly GetRelatorioFluxoCaixaQueryHandler _handler;

        public GetRelatorioFluxoCaixaQueryHandlerTests()
        {
            _mockSaldoDiarioRepository = Substitute.For<ISaldoDiarioRepository>();
            _mockLogger = Substitute.For<ILogger<GetRelatorioFluxoCaixaQueryHandler>>();
            _handler = new GetRelatorioFluxoCaixaQueryHandler(
                _mockSaldoDiarioRepository,
                _mockLogger
            );
        }

        /// <summary>
        /// Testa se o handler calcula corretamente o relatório para um período com dados.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-002: Verificação do cálculo do relatório de fluxo de caixa.
        /// HU-007: Cenário de sucesso com dados.
        /// TDD: Cenário de sucesso.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-002, HU-007")]
        public async Task Handle_PeriodWithData_ReturnsCorrectRelatorio()
        {
            // Arrange
            var dataInicio = (DateTime.Today.AddDays(-2));
            var dataFim = (DateTime.Today);
            var query = new GetRelatorioFluxoCaixaQuery(dataInicio, dataFim);

            var saldoDiarioDiaMenosUm = SaldoDiario.Create(dataInicio.AddDays(-1));
            saldoDiarioDiaMenosUm.AplicarLancamento(TipoLancamento.Credito.ToString(), 100.00m); // Credito 100
            saldoDiarioDiaMenosUm.AplicarLancamento(TipoLancamento.Debito.ToString(), 20.00m); // Debito 20

            // SaldoTotal: 80.00 (Saldo Inicial para o período)

            var saldoDiarioDiaUm = SaldoDiario.Create(dataInicio);
            saldoDiarioDiaUm.AplicarLancamento(TipoLancamento.Credito.ToString(), 50.00m); // Credito 50
            saldoDiarioDiaUm.AplicarLancamento(TipoLancamento.Debito.ToString(), 10.00m); // Debito 10

            var saldoDiarioDiaDois = SaldoDiario.Create(dataInicio.AddDays(1));
            // saldoDiarioDiaDois.AplicarLancamento(30.00m, TipoLancamento.Credito); // Credito 30
            // saldoDiarioDiaDois.AplicarLancamento(15.00m, TipoLancamento.Debito); // Debito 15
            saldoDiarioDiaDois.AplicarLancamento(TipoLancamento.Credito.ToString(), 30.00m); // Credito 30
            saldoDiarioDiaDois.AplicarLancamento(TipoLancamento.Debito.ToString(), 15.00m); // Debito 15

            var saldoDiarioDiaTres = SaldoDiario.Create(dataFim);
            // saldoDiarioDiaTres.AplicarLancamento(70.00m, TipoLancamento.Credito); // Credito 70
            // saldoDiarioDiaTres.AplicarLancamento(25.00m, TipoLancamento.Debito); // Debito 25
            saldoDiarioDiaTres.AplicarLancamento(TipoLancamento.Credito.ToString(), 70.00m); // Credito 70
            saldoDiarioDiaTres.AplicarLancamento(TipoLancamento.Debito.ToString(), 25.00m);

            var saldosNoPeriodo = new List<SaldoDiario>
            {
                saldoDiarioDiaUm,
                saldoDiarioDiaDois,
                saldoDiarioDiaTres
            };

            _mockSaldoDiarioRepository.GetByDateAsync(dataInicio.AddDays(-1)).Returns(saldoDiarioDiaMenosUm);
            _mockSaldoDiarioRepository.GetByDateRangeAsync(dataInicio, dataFim).Returns(saldosNoPeriodo);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.DataInicio.Should().Be(dataInicio);
            result.DataFim.Should().Be(dataFim);
            result.SaldoInicial.Should().Be(80.00m); // Saldo do dia anterior
            result.TotalCreditos.Should().Be(50m + 30m + 70m); // 150.00
            result.TotalDebitos.Should().Be(10m + 15m + 25m);   // 50.00
            result.SaldoFinal.Should().Be(80.00m + 150.00m - 50.00m); // 180.00

            _mockSaldoDiarioRepository.Received(1).GetByDateAsync(dataInicio.AddDays(-1));
            _mockSaldoDiarioRepository.Received(1).GetByDateRangeAsync(dataInicio, dataFim);
        }

        /// <summary>
        /// Testa se o handler retorna um relatório com valores zerados para um período sem dados.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-002: Tratamento de cenário sem dados.
        /// HU-007: Retorno vazio para período sem lançamentos.
        /// TDD: Cenário de sucesso - sem dados.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-002, HU-007")]
        public async Task Handle_PeriodWithoutData_ReturnsZeroedRelatorio()
        {
            // Arrange
            var dataInicio = (DateTime.Today.AddDays(-2));
            var dataFim = (DateTime.Today);
            var query = new GetRelatorioFluxoCaixaQuery(dataInicio, dataFim);

            // Simula que não há saldo para o dia anterior
            _mockSaldoDiarioRepository.GetByDateAsync(dataInicio.AddDays(-1)).Returns((SaldoDiario)null);
            // Simula que não há saldos no período
            _mockSaldoDiarioRepository.GetByDateRangeAsync(dataInicio, dataFim).Returns(new List<SaldoDiario>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.DataInicio.Should().Be(dataInicio);
            result.DataFim.Should().Be(dataFim);
            result.SaldoInicial.Should().Be(0m);
            result.TotalCreditos.Should().Be(0m);
            result.TotalDebitos.Should().Be(0m);
            result.SaldoFinal.Should().Be(0m);
        }

        /// <summary>
        /// Testa se o handler lida com exceções durante a busca de dados para o relatório.
        /// </summary>
        /// <remarks>
        /// NFR-RES-001: Teste de resiliência e tratamento de erros.
        /// TDD: Cenário de falha - tratamento de exceção.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, NFR-RES-001")]
        public async Task Handle_RepositoryThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var dataInicio = (DateTime.Today.AddDays(-2));
            var dataFim = (DateTime.Today);
            var query = new GetRelatorioFluxoCaixaQuery(dataInicio, dataFim);

            _mockSaldoDiarioRepository
                .GetByDateRangeAsync(dataInicio, dataFim)
                .Throws(new InvalidOperationException("Erro de banco de dados simulado."));

            var context = new CancellationToken();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));

            _mockSaldoDiarioRepository.Received(1).GetByDateRangeAsync(dataInicio, dataFim);
            _mockSaldoDiarioRepository.DidNotReceive().GetByDateAsync(Arg.Any<DateTime>()); // Não deve tentar buscar saldo inicial se falhou antes
        }
    }
}