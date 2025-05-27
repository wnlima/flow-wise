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
    /// Contém testes unitários para o consumidor de eventos <see cref="LancamentoAtualizadoEventConsumer"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes verificam se o consumidor processa corretamente os eventos de lançamento atualizado,
    /// ajustando os <see cref="SaldoDiario"/>s correspondentes (tanto da data antiga quanto da nova, se houver mudança de data),
    /// utilizando NSubstitute para mocks.
    /// REQ-FLW-CON-001: Consolidar lançamentos diários com atualização.
    /// NFR-RES-001: O serviço de consolidação deve ser resiliente a indisponibilidades.
    /// HU-006: Como um usuário, eu quero ver o saldo total de um dia específico.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class LancamentoAtualizadoEventConsumerTests
    {
        private readonly ISaldoDiarioRepository _mockSaldoDiarioRepository;
        private readonly ILogger<LancamentoAtualizadoEventConsumer> _mockLogger;
        private readonly LancamentoAtualizadoEventConsumer _consumer;

        public LancamentoAtualizadoEventConsumerTests()
        {
            _mockSaldoDiarioRepository = Substitute.For<ISaldoDiarioRepository>(); // Criando mock com NSubstitute
            _mockLogger = Substitute.For<ILogger<LancamentoAtualizadoEventConsumer>>(); // Criando mock com NSubstitute
            _consumer = new LancamentoAtualizadoEventConsumer(
                _mockSaldoDiarioRepository,
                _mockLogger
            );
        }

        /// <summary>
        /// Testa a atualização de um lançamento que permanece na mesma data, ajustando os valores do saldo diário.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a correção do saldo diário após atualização do lançamento na mesma data.
        /// TDD: Cenário de sucesso - atualização de valor/tipo na mesma data.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_LancamentoUpdatedSameDay_AdjustsSaldoDiarioCorrectly()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var data = DateTime.Today;

            var saldoDiario = SaldoDiario.Create(data);
            saldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), 100.00m); // Lançamento antigo
            saldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), 50.00m); // Outro lançamento
            var initialTotalCreditos = saldoDiario.TotalCreditos; // Deve ser 100
            var initialTotalDebitos = saldoDiario.TotalDebitos;   // Deve ser 50
            var initialSaldoTotal = saldoDiario.SaldoTotal;       // Deve ser 50

            var oldLancamentoSnapshot = new LancamentoSnapshot(lancamentoId, 100.00m, TipoLancamento.Credito.ToString(), data, "old", CategoriaLancamento.Alimentacao.ToString(), null);
            var newLancamento = new LancamentoSnapshot(lancamentoId, 120.00m, TipoLancamento.Credito.ToString(), data, "new", CategoriaLancamento.Alimentacao.ToString(), null);
            var e = new LancamentoAtualizadoEvent(lancamentoId, oldLancamentoSnapshot, newLancamento, Guid.NewGuid().ToString());

            _mockSaldoDiarioRepository
                .GetByDateAsync(data)
                .Returns(saldoDiario);

            var context = Substitute.For<ConsumeContext<LancamentoAtualizadoEvent>>();
            context.Message.Returns(e);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica se UpdateAsync foi chamado com o SaldoDiario atualizado
            await _mockSaldoDiarioRepository.Received(2).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == data &&
                s.TotalCreditos == (initialTotalCreditos - oldLancamentoSnapshot.Valor + newLancamento.Valor) && // 100 - 100 + 120 = 120
                s.TotalDebitos == initialTotalDebitos && // 50
                s.SaldoTotal == (initialSaldoTotal - oldLancamentoSnapshot.Valor + newLancamento.Valor) // 50 - 100 + 120 = 70
            ));
            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();
        }

        /// <summary>
        /// Testa a atualização de um lançamento que muda para uma nova data, ajustando os saldos diários de ambas as datas.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a correção do saldo diário após atualização do lançamento com mudança de data.
        /// TDD: Cenário de sucesso - mudança de data do lançamento.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_LancamentoUpdatedToNewDay_AdjustsBothSaldoDiariosCorrectly()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var oldData = DateTime.Today.AddDays(-1);
            var newData = DateTime.Today;

            var oldSaldoDiario = SaldoDiario.Create(oldData);
            oldSaldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), 200.00m); // Lançamento antigo
            oldSaldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), 100.00m); // Outro lançamento
            var oldInitialTotalCreditos = oldSaldoDiario.TotalCreditos; // 200
            var oldInitialTotalDebitos = oldSaldoDiario.TotalDebitos;   // 100
            var oldInitialSaldoTotal = oldSaldoDiario.SaldoTotal;       // 100

            var newSaldoDiario = SaldoDiario.Create(newData);
            newSaldoDiario.AplicarLancamento(TipoLancamento.Credito.ToString(), 50.00m); // Lançamento na nova data
            var newInitialTotalCreditos = newSaldoDiario.TotalCreditos; // 50
            var newInitialTotalDebitos = newSaldoDiario.TotalDebitos;   // 0
            var newInitialSaldoTotal = newSaldoDiario.SaldoTotal;       // 50

            var oldLancamentoSnapshot = new LancamentoSnapshot(lancamentoId, 150.00m, TipoLancamento.Credito.ToString(), oldData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var newLancamento = new LancamentoSnapshot(lancamentoId, 170.00m, TipoLancamento.Credito.ToString(), newData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var e = new LancamentoAtualizadoEvent(lancamentoId, oldLancamentoSnapshot, newLancamento, Guid.NewGuid().ToString());


            _mockSaldoDiarioRepository
                .GetByDateAsync(oldData)
                .Returns(oldSaldoDiario);
            _mockSaldoDiarioRepository
                .GetByDateAsync(newData)
                .Returns(newSaldoDiario);

            var context = Substitute.For<ConsumeContext<LancamentoAtualizadoEvent>>();
            context.Message.Returns(e);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica o saldo diário antigo foi atualizado
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == oldData &&
                s.TotalCreditos == (oldInitialTotalCreditos - oldLancamentoSnapshot.Valor) && // 200 - 150 = 50
                s.TotalDebitos == oldInitialTotalDebitos && // 100
                s.SaldoTotal == (oldInitialSaldoTotal - oldLancamentoSnapshot.Valor) // 100 - 150 = -50
            ));

            // Verifica o novo saldo diário foi atualizado
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == newData &&
                s.TotalCreditos == (newInitialTotalCreditos + newLancamento.Valor) && // 50 + 170 = 220
                s.TotalDebitos == newInitialTotalDebitos && // 0
                s.SaldoTotal == (newInitialSaldoTotal + newLancamento.Valor) // 50 + 170 = 220
            ));

            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();
        }

        /// <summary>
        /// Testa a atualização de um lançamento que muda para uma nova data onde o SaldoDiario ainda não existe.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a criação de SaldoDiario para a nova data, se necessário, ao mover um lançamento.
        /// TDD: Cenário de sucesso - mudança de data para um dia sem SaldoDiario.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Consume_LancamentoUpdatedToNewDay_CreatesNewSaldoDiarioIfNotExist()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var oldData = DateTime.Today.AddDays(-1);
            var newData = DateTime.Today;

            var oldSaldoDiario = SaldoDiario.Create(oldData);
            oldSaldoDiario.AplicarLancamento(TipoLancamento.Debito.ToString(), 100.00m); // Lançamento antigo para ser revertido
            var oldInitialTotalDebitos = oldSaldoDiario.TotalDebitos; // 100
            var oldInitialSaldoTotal = oldSaldoDiario.SaldoTotal;       // -100

            var oldLancamentoSnapshot = new LancamentoSnapshot(lancamentoId, 100.00m, TipoLancamento.Debito.ToString(), oldData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var newLancamento = new LancamentoSnapshot(lancamentoId, 50.00m, TipoLancamento.Credito.ToString(), newData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var e = new LancamentoAtualizadoEvent(lancamentoId, oldLancamentoSnapshot, newLancamento, Guid.NewGuid().ToString());

            _mockSaldoDiarioRepository
                .GetByDateAsync(oldData)
                .Returns(oldSaldoDiario);
            _mockSaldoDiarioRepository
                .GetByDateAsync(newData)
                .Returns((SaldoDiario)null); // Simula que não há saldo para a nova data

            var context = Substitute.For<ConsumeContext<LancamentoAtualizadoEvent>>();
            context.Message.Returns(e);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Verifica o saldo diário antigo foi atualizado
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == oldData &&
                s.TotalDebitos == (oldInitialTotalDebitos - oldLancamentoSnapshot.Valor) && // 100 - 100 = 0
                s.SaldoTotal == (oldInitialSaldoTotal + oldLancamentoSnapshot.Valor) // -100 + 100 = 0
            ));

            // Verifica a criação e aplicação no novo saldo diário
            await _mockSaldoDiarioRepository.Received(1).AddAsync(Arg.Is<SaldoDiario>(s =>
                s.Data == newData &&
                s.TotalCreditos == newLancamento.Valor &&
                s.TotalDebitos == 0 &&
                s.SaldoTotal == newLancamento.Valor
            ));

            await _mockSaldoDiarioRepository.Received(1).UnitOfWork();
        }

        /// <summary>
        /// Testa se o consumidor lida com o cenário onde o saldo diário antigo não é encontrado (possível inconsistência).
        /// </summary>
        /// <remarks>
        /// NFR-RES-001: Resiliência a inconsistências e logging de avisos.
        /// TDD: Cenário de falha - SaldoDiario antigo não encontrado.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, NFR-RES-001")]
        public async Task Consume_OldSaldoDiarioNotFound_LogsWarningAndContinues()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var oldData = DateTime.Today.AddDays(-1);
            var newData = DateTime.Today;

            var newSaldoDiario = SaldoDiario.Create(newData);
            var oldLancamentoSnapshot = new LancamentoSnapshot(lancamentoId, 100.00m, TipoLancamento.Credito.ToString(), oldData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var newLancamento = new LancamentoSnapshot(lancamentoId, 120.00m, TipoLancamento.Credito.ToString(), newData, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var e = new LancamentoAtualizadoEvent(lancamentoId, oldLancamentoSnapshot, newLancamento, Guid.NewGuid().ToString());

            _mockSaldoDiarioRepository
                .GetByDateAsync(oldData)
                .Returns((SaldoDiario)null); // Simula que o saldo antigo não existe
            _mockSaldoDiarioRepository
                .GetByDateAsync(newData)
                .Returns(newSaldoDiario);

            var context = Substitute.For<ConsumeContext<LancamentoAtualizadoEvent>>();
            context.Message.Returns(e);

            // Act
            await _consumer.Consume(context);

            // Assert
            // Apenas o novo saldo será atualizado (porque foi encontrado)
            await _mockSaldoDiarioRepository.Received(1).UpdateAsync(Arg.Is<SaldoDiario>(s => s.Data == newData));
            // AddAsync não deve ser chamado, pois o novo SaldoDiario já existia
            await _mockSaldoDiarioRepository.DidNotReceive().AddAsync(Arg.Any<SaldoDiario>());
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
            var lancamentoId = Guid.NewGuid();
            var data = DateTime.Today;

            var oldLancamentoSnapshot = new LancamentoSnapshot(lancamentoId, 100.00m, TipoLancamento.Credito.ToString(), data, "old", CategoriaLancamento.Alimentacao.ToString(), "");
            var newLancamento = new LancamentoSnapshot(lancamentoId, 120.00m, TipoLancamento.Credito.ToString(), data, "new", CategoriaLancamento.Alimentacao.ToString(), "");
            var e = new LancamentoAtualizadoEvent(lancamentoId, oldLancamentoSnapshot, newLancamento, Guid.NewGuid().ToString());

            _mockSaldoDiarioRepository
                .GetByDateAsync(Arg.Any<DateTime>())
                .Throws(new InvalidOperationException("Erro de banco de dados simulado."));

            var context = Substitute.For<ConsumeContext<LancamentoAtualizadoEvent>>();
            context.Message.Returns(e);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _consumer.Consume(context));

            // Verifica que AddAsync ou UpdateAsync não foram chamados
            await _mockSaldoDiarioRepository.DidNotReceive().UpdateAsync(Arg.Any<SaldoDiario>());
            await _mockSaldoDiarioRepository.DidNotReceive().AddAsync(Arg.Any<SaldoDiario>());
            await _mockSaldoDiarioRepository.DidNotReceive().UnitOfWork();
        }
    }
}