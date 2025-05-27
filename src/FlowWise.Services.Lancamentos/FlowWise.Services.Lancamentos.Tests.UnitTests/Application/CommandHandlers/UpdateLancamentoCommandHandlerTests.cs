using FluentAssertions;
using NSubstitute;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.CommandHandlers;
using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.CommandHandlers
{
    public class UpdateLancamentoCommandHandlerTests
    {
        private readonly ILancamentoRepository _mockLancamentoRepository;
        private readonly IDomainEventPublisher _mockDomainEventPublisher;
        private readonly ILogger<UpdateLancamentoCommandHandler> _mockLogger;
        private readonly UpdateLancamentoCommandHandler _handler;

        public UpdateLancamentoCommandHandlerTests()
        {
            _mockLancamentoRepository = Substitute.For<ILancamentoRepository>();
            _mockDomainEventPublisher = Substitute.For<IDomainEventPublisher>();
            _mockLogger = Substitute.For<ILogger<UpdateLancamentoCommandHandler>>();

            _handler = new UpdateLancamentoCommandHandler(_mockLancamentoRepository, _mockDomainEventPublisher, _mockLogger);
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "HU-004, NFR-OBS-001, NFR-RES-004, TDD")] // Adicionado HU-004 e NFR-RES-004
        public async Task Handle_UpdateLancamentoCommand_ShouldUpdateLancamentoAndPublishEventWithOldAndNewValues()
        {
            // Arrange
            var initialValor = 100m;
            var initialData = DateTime.Today.AddDays(-5);
            var initialDescricao = "Lancamento Antigo";
            var initialTipo = "Credito";
            var initialCategoria = "Salario";
            var initialObservacoes = "Observacao Inicial";

            var existingLancamento = Lancamento.Create(
                initialValor, initialData, initialDescricao, initialTipo, initialCategoria, initialObservacoes, "initial-corr-id"
            );
            var lancamentoId = existingLancamento.Id;
            existingLancamento.ClearDomainEvents();
            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);
            _mockLancamentoRepository.UnitOfWork().Returns(1);

            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = lancamentoId,
                Valor = 150.75m,
                Data = DateTime.Today.AddDays(-2),
                Descricao = "Lancamento Atualizado",
                Tipo = "Credito", // Mantendo o tipo como "Credito" conforme RN-005
                Categoria = "Alimentacao",
                Observacoes = "Nova Observacao"
            };

            Func<LancamentoAtualizadoEvent, bool> checkEvent = updatedEvent =>
                                       updatedEvent != null &&
                                      updatedEvent.LancamentoId == lancamentoId &&
                                      updatedEvent.LancamentoAntigo.Id == lancamentoId &&
                                      updatedEvent.LancamentoAntigo.Valor == initialValor &&
                                      updatedEvent.LancamentoAntigo.Data.Date == initialData.Date &&
                                      updatedEvent.LancamentoAntigo.Tipo == initialTipo &&
                                      updatedEvent.LancamentoAntigo.Descricao == initialDescricao &&
                                      updatedEvent.LancamentoAntigo.Categoria == initialCategoria &&
                                      updatedEvent.LancamentoAntigo.Observacoes == initialObservacoes &&
                                      updatedEvent.LancamentoNovo.Id == lancamentoId &&
                                      updatedEvent.LancamentoNovo.Valor == command.Valor &&
                                      updatedEvent.LancamentoNovo.Data.Date == command.Data.Date &&
                                      updatedEvent.LancamentoNovo.Tipo == command.Tipo &&
                                      updatedEvent.LancamentoNovo.Descricao == command.Descricao &&
                                      updatedEvent.LancamentoNovo.Categoria == command.Categoria &&
                                      updatedEvent.LancamentoNovo.Observacoes == command.Observacoes &&
                                      updatedEvent.CorrelationId == command.CorrelationId;

            var eventPulbished = false;
            _mockDomainEventPublisher.Publish(Arg.Do<IEnumerable<IDomainEvent>>(e => eventPulbished = e.Any(x => checkEvent(x as LancamentoAtualizadoEvent))))
                                     .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            await _mockLancamentoRepository.Received(1).GetByIdAsync(lancamentoId);

            await _mockLancamentoRepository.Received(1).UpdateAsync(Arg.Is<Lancamento>(l =>
                l.Id == lancamentoId
            ));

            await _mockLancamentoRepository.Received(1).UnitOfWork();

            eventPulbished.Should().BeTrue();

            var message = $"Lancamento with ID {lancamentoId} updated successfully (Rows affected: {1}). CorrelationId: {command.CorrelationId}";
            _mockLogger.ReceivedCalls()
                            .Select(call => call.GetArguments())
                            .Select(callArguments => callArguments[2].ToString()).Should().Contain(message);
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "RN-005")]
        public async Task Handle_UpdateLancamentoCommand_ShouldThrowExceptionIfTipoIsChanged()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var existingLancamento = Lancamento.Create(100m, DateTime.Today.AddDays(-5), "Original", "Credito", "Salario", null, "initial-corr-id");
            existingLancamento.ClearDomainEvents();

            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);

            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = lancamentoId,
                Valor = 150m,
                Data = DateTime.Today,
                Descricao = "Atualizado",
                Tipo = "Debito", // Tentando mudar o tipo de Credito para Debito
                Categoria = "Outros"
            };

            // Act & Assert
            await FluentActions.Awaiting(() => _handler.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<LancamentoDomainException>()
                .WithMessage("O tipo de um lançamento não pode ser alterado após o registro. [RN-005]");

            await _mockLancamentoRepository.DidNotReceive().UpdateAsync(Arg.Any<Lancamento>());
            await _mockLancamentoRepository.DidNotReceive().UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        public async Task Handle_UpdateLancamentoCommand_ShouldReturnFalseIfLancamentoNotFound()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns((Lancamento)null);

            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = lancamentoId,
                Valor = 150m,
                Data = DateTime.Today,
                Descricao = "Atualizado",
                Tipo = "Credito",
                Categoria = "Outros"
            };

            // Act & Assert
            await FluentActions.Awaiting(() => _handler.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<LancamentoDomainException>()
                .WithMessage($"Lançamento com ID '{lancamentoId}' não encontrado.");

            await _mockLancamentoRepository.DidNotReceive().UpdateAsync(Arg.Any<Lancamento>());
            await _mockLancamentoRepository.DidNotReceive().UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());

            var message = $"Lancamento with ID {lancamentoId} not found for update. CorrelationId: {command.CorrelationId}";
            _mockLogger.ReceivedCalls()
                            .Select(call => call.GetArguments())
                            .Select(callArguments => callArguments[2].ToString()).Should().Contain(message);
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        public async Task Handle_UpdateLancamentoCommand_ShouldNotPublishEventIfNoRowsAffected()
        {
            // Arrange
            var existingLancamento = Lancamento.Create(100m, DateTime.Today.AddDays(-5), "Original", "Credito", "Salario", null, "initial-corr-id");
            existingLancamento.ClearDomainEvents();
            var lancamentoId = existingLancamento.Id;

            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);
            _mockLancamentoRepository.UnitOfWork().Returns(0);

            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = lancamentoId,
                Valor = 100m,
                Data = DateTime.Today.AddDays(-5),
                Descricao = "Original",
                Tipo = "Credito",
                Categoria = "Salario",
                Observacoes = null
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            await _mockLancamentoRepository.Received(1).UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());

            var message = $"Lancamento with ID {lancamentoId} updated successfully (Rows affected: {0}). CorrelationId: {command.CorrelationId}";
            _mockLogger.ReceivedCalls()
                            .Select(call => call.GetArguments())
                            .Select(callArguments => callArguments[2].ToString()).Should().Contain(message);
        }
    }
}