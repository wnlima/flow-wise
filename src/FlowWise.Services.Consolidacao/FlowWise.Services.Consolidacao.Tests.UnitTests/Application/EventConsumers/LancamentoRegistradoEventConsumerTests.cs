using FlowWise.Services.Consolidacao.Application.EventConsumers;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FlowWise.Services.Consolidacao.Tests.UnitTests.Application.EventConsumers
{
    /// <summary>
    /// Contém testes unitários para o consumidor de eventos <see cref="LancamentoRegistradoEventConsumer"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes verificam se o consumidor processa corretamente os eventos de lançamento registrado,
    /// criando ou atualizando o <see cref="SaldoDiario"/> correspondente, utilizando NSubstitute para mocks.
    /// REQ-FLW-CON-001: Consolidar lançamentos diários.
    /// NFR-RES-001: O serviço de consolidação deve ser resiliente a indisponibilidades do serviço de lançamentos.
    /// HU-006: Como um usuário, eu quero ver o saldo total de um dia específico.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class LancamentoRegistradoEventConsumerTests
    {
        private readonly ISaldoDiarioRepository _mockSaldoDiarioRepository;
        private readonly ILogger<LancamentoRegistradoEventConsumer> _mockLogger;
        private readonly LancamentoRegistradoEventConsumer _consumer;

        public LancamentoRegistradoEventConsumerTests()
        {
            _mockSaldoDiarioRepository = Substitute.For<ISaldoDiarioRepository>(); // Criando mock com NSubstitute
            _mockLogger = Substitute.For<ILogger<LancamentoRegistradoEventConsumer>>(); // Criando mock com NSubstitute
            _consumer = new LancamentoRegistradoEventConsumer(
                _mockSaldoDiarioRepository,
                _mockLogger
            );
        }

        /// <summary>
        /// Testa se o consumidor cria um novo <see cref="SaldoDiario"/> quando nenhum existe para a data do lançamento.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a criação de um novo SaldoDiario ao registrar o primeiro lançamento do dia.
        /// TDD: Cenário de sucesso - novo SaldoDiario.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_NewLancamentoForNewDay_CreatesNewSaldoDiario()
        {
            // Arrange
            var dataLancamento = DateTime.Today;
            // var lancamentoEvent = new LancamentoRegistradoEvent(
            //     Guid.NewGuid(),
            //     100.00m,
            //     TipoLancamento.Credito,
            //     dataLancamento,
            //     "Compras",
            //     "Alimentos",
            //     Guid.NewGuid()
            // );

            var lancamentoEvent = new LancamentoRegistradoEvent();
            lancamentoEvent.LancamentoId = Guid.NewGuid();
            lancamentoEvent.Valor = 100.00m;
            lancamentoEvent.Tipo = TipoLancamento.Credito.ToString();
            lancamentoEvent.Data = dataLancamento;

            // Configura o mock para retornar null, simulando que não há saldo para a data
            _mockSaldoDiarioRepository.GetByDateAsync(dataLancamento).Returns((SaldoDiario)null);

            // Usando NSubstitute para criar um ConsumeContext
            var context = Substitute.For<ConsumeContext<LancamentoRegistradoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica se AddAsync foi chamado com um SaldoDiario que corresponde aos dados esperados
            await _mockSaldoDiarioRepository.Received(1).AddAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == dataLancamento &&
                s.TotalCreditos == lancamentoEvent.Valor &&
                s.TotalDebitos == 0 &&
                s.SaldoTotal == lancamentoEvent.Valor
            ));

            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();
        }

        /// <summary>
        /// Testa se o consumidor atualiza um <see cref="SaldoDiario"/> existente para a data do lançamento.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a atualização de um SaldoDiario existente ao registrar novo lançamento no mesmo dia.
        /// TDD: Cenário de sucesso - SaldoDiario existente.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_NewLancamentoForExistingDay_UpdatesExistingSaldoDiario()
        {
            // Arrange
            var dataLancamento = DateTime.Today;
            var existingSaldoDiario = SaldoDiario.Create(dataLancamento);
            existingSaldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), 50.00m); // Saldo inicial
            var initialTotalCreditos = existingSaldoDiario.TotalCreditos;
            var initialSaldoTotal = existingSaldoDiario.SaldoTotal;

            var lancamentoEvent = new LancamentoRegistradoEvent();
            lancamentoEvent.LancamentoId = Guid.NewGuid();
            lancamentoEvent.Valor = 75.00m;
            lancamentoEvent.Tipo = TipoLancamento.Debito.ToString();
            lancamentoEvent.Data = dataLancamento;

            _mockSaldoDiarioRepository.GetByDateAsync(dataLancamento).Returns(existingSaldoDiario);

            var context = Substitute.For<ConsumeContext<LancamentoRegistradoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica se UpdateAsync foi chamado com um SaldoDiario com os valores atualizados
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == dataLancamento &&
                s.TotalCreditos == initialTotalCreditos &&
                s.TotalDebitos == lancamentoEvent.Valor && // Novo débito de 75.00
                s.SaldoTotal == (initialSaldoTotal - lancamentoEvent.Valor) // 50.00 - 75.00 = -25.00
            ));
            // Verifica se SaveEntitiesAsync foi chamado
            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();

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
            var dataLancamento = DateTime.Today;
            var lancamentoEvent = new LancamentoRegistradoEvent();
            lancamentoEvent.LancamentoId = Guid.NewGuid();
            lancamentoEvent.Valor = 100.00m;
            lancamentoEvent.Tipo = TipoLancamento.Credito.ToString();
            lancamentoEvent.Data = dataLancamento;

            // Configura o mock para lançar uma exceção ao tentar buscar o saldo
            _mockSaldoDiarioRepository
                .GetByDateAsync(dataLancamento)
                .Throws(new InvalidOperationException("Erro de banco de dados simulado."));

            var context = Substitute.For<ConsumeContext<LancamentoRegistradoEvent>>();
            context.Message.Returns(lancamentoEvent);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(context));

            // Verifica que AddAsync ou UpdateAsync não foram chamados
            await _mockSaldoDiarioRepository.DidNotReceive().AddAsync(Arg.Any<SaldoDiario>());
            await _mockSaldoDiarioRepository.DidNotReceive().UpdateAsync(Arg.Any<SaldoDiario>());
            // Verifica que SaveEntitiesAsync não foi chamado
            await _mockSaldoDiarioRepository.DidNotReceive().UnitOfWork();
        }
    }
}