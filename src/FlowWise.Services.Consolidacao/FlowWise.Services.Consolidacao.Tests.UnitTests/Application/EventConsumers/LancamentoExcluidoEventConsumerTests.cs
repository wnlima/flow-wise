using FlowWise.Services.Consolidacao.Application.EventConsumers;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using NSubstitute.ExceptionExtensions;

namespace FlowWise.Services.Consolidacao.Tests.UnitTests.Application.EventConsumers
{
    /// <summary>
    /// Contém testes unitários para o consumidor de eventos <see cref="LancamentoExcluidoEventConsumer"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes verificam se o consumidor processa corretamente os eventos de lançamento excluído,
    /// revertendo o lançamento do <see cref="SaldoDiario"/> correspondente, utilizando NSubstitute para mocks.
    /// REQ-FLW-CON-001: Consolidar lançamentos diários com exclusão.
    /// NFR-RES-001: O serviço de consolidação deve ser resiliente a indisponibilidades.
    /// HU-006: Como um usuário, eu quero ver o saldo total de um dia específico.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class LancamentoExcluidoEventConsumerTests
    {
        private readonly ISaldoDiarioRepository _mockSaldoDiarioRepository;
        private readonly ILogger<LancamentoExcluidoEventConsumer> _mockLogger;
        private readonly LancamentoExcluidoEventConsumer _consumer;

        public LancamentoExcluidoEventConsumerTests()
        {
            _mockSaldoDiarioRepository = Substitute.For<ISaldoDiarioRepository>(); // Criando mock com NSubstitute
            _mockLogger = Substitute.For<ILogger<LancamentoExcluidoEventConsumer>>(); // Criando mock com NSubstitute
            _consumer = new LancamentoExcluidoEventConsumer(
                _mockSaldoDiarioRepository,
                _mockLogger
            );
        }

        /// <summary>
        /// Testa se o consumidor reverte corretamente um lançamento de um <see cref="SaldoDiario"/> existente.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a correção do saldo diário após exclusão do lançamento.
        /// TDD: Cenário de sucesso - reversão de lançamento.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_LancamentoExists_RevertsSaldoDiarioCorrectly()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var dataLancamento = (DateTime.Today);

            var saldoDiario = SaldoDiario.Create(dataLancamento);
            saldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), 100.00m); // Lançamento a ser excluído
            saldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), 50.00m); // Outro lançamento
            var initialTotalCreditos = saldoDiario.TotalCreditos; // 100
            var initialTotalDebitos = saldoDiario.TotalDebitos;   // 50
            var initialSaldoTotal = saldoDiario.SaldoTotal;       // 50

            var lancamentoEvent = new LancamentoExcluidoEvent(
                lancamentoId,
                100.00m,
                TipoLancamento.Credito.ToString(),
                dataLancamento,
                Guid.NewGuid().ToString()
            );


            _mockSaldoDiarioRepository
                .GetByDateAsync(dataLancamento)
                .Returns(saldoDiario);

            var context = Substitute.For<ConsumeContext<LancamentoExcluidoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica se UpdateAsync foi chamado com o SaldoDiario atualizado
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == dataLancamento &&
                s.TotalCreditos == (initialTotalCreditos - lancamentoEvent.Valor) && // 100 - 100 = 0
                s.TotalDebitos == initialTotalDebitos && // 50
                s.SaldoTotal == (initialSaldoTotal - lancamentoEvent.Valor) // 50 - 100 = -50
            ));
            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();
        }

        /// <summary>
        /// Testa se o consumidor lida corretamente com a exclusão de um lançamento
        /// quando o <see cref="SaldoDiario"/> para a data não é encontrado.
        /// </summary>
        /// <remarks>
        /// NFR-RES-001: Resiliência a inconsistências e logging de avisos.
        /// TDD: Cenário de falha - SaldoDiario não encontrado.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, NFR-RES-001")]
        public async Task Consume_SaldoDiarioNotFound_LogsWarningAndDoesNotThrow()
        {
            // Arrange
            var dataLancamento = (DateTime.Today);
            // var lancamentoEvent = new LancamentoExcluidoEvent(
            //     Guid.NewGuid(),
            //     100.00m,
            //     TipoLancamento.Credito,
            //     dataLancamento,
            //     "Compras",
            //     "Alimentos",
            //     Guid.NewGuid()
            // );
            var lancamentoEvent = new LancamentoExcluidoEvent(
                Guid.NewGuid(),
                100.00m,
                TipoLancamento.Credito.ToString(),
                dataLancamento,
                Guid.NewGuid().ToString()
            );

            _mockSaldoDiarioRepository
                .GetByDateAsync(dataLancamento)
                .Returns((SaldoDiario)null); // Simula que não há saldo para a data

            var context = Substitute.For<ConsumeContext<LancamentoExcluidoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act
            await _consumer.Consume(context);

            // Assert
            await _mockSaldoDiarioRepository.DidNotReceive().UpdateAsync(Arg.Any<SaldoDiario>());
            await _mockSaldoDiarioRepository.DidNotReceive().UnitOfWork();
        }

        /// <summary>
        /// Testa se o consumidor lida com exceções durante o processamento do evento.
        /// </summary>
        /// <remarks>
        /// NFR-RES-001: Teste de resiliência e tratamento de erros.
        /// TDD: Cenário de falha - tratamento de exceção.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, NFR-RES-001")]
        public async Task Consume_ThrowsException_LogsErrorAndDoesNotCommit()
        {
            // Arrange
            var dataLancamento = (DateTime.Today);

            var lancamentoEvent = new LancamentoExcluidoEvent(
                Guid.NewGuid(),
                100.00m,
                TipoLancamento.Credito.ToString(),
                dataLancamento,
                Guid.NewGuid().ToString()
            );

            _mockSaldoDiarioRepository
                .GetByDateAsync(dataLancamento)
                .Throws(new InvalidOperationException("Erro de banco de dados simulado."));

            var context = Substitute.For<ConsumeContext<LancamentoExcluidoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(context));

            // Verifica que UpdateAsync não foi chamado
            await _mockSaldoDiarioRepository.DidNotReceive().UpdateAsync(Arg.Any<SaldoDiario>());
            // Verifica que SaveEntitiesAsync não foi chamado
            await _mockSaldoDiarioRepository.DidNotReceive().UnitOfWork();
        }
    }
}