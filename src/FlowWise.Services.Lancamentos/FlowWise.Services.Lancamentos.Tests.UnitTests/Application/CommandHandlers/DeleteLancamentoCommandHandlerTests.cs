using FluentAssertions;
using NSubstitute;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.CommandHandlers;
using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Events;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Lancamentos.Domain.Exceptions;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.CommandHandlers
{
    public class DeleteLancamentoCommandHandlerTests
    {
        private readonly ILancamentoRepository _mockLancamentoRepository;
        private readonly IDomainEventPublisher _mockDomainEventPublisher;
        private readonly ILogger<DeleteLancamentoCommandHandler> _mockLogger;
        private readonly DeleteLancamentoCommandHandler _handler;

        public DeleteLancamentoCommandHandlerTests()
        {
            _mockLancamentoRepository = Substitute.For<ILancamentoRepository>();
            _mockDomainEventPublisher = Substitute.For<IDomainEventPublisher>();
            _mockLogger = Substitute.For<ILogger<DeleteLancamentoCommandHandler>>();

            _handler = new DeleteLancamentoCommandHandler(_mockLancamentoRepository, _mockDomainEventPublisher, _mockLogger);
        }

        [Fact(DisplayName = "DeleteLancamento_Should_Mark_As_Deleted_And_Publish_Event_On_Success")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "HU-005, RN-007, NFR-OBS-001, NFR-SEG-005, TDD")]
        public async Task Handle_DeleteLancamentoCommand_ShouldMarkAsDeletedAndPublishEvent()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();
            var existingLancamento = Lancamento.Create(
                100m,
                DateTime.Today, // Importante: RN-001 - Lançamentos só podem ser excluídos se a data for igual ao dia atual (D-0)
                "Lançamento a ser Excluído",
                "Debito",
                "Lazer",
                "Lançamento de teste",
                "original-corr-id"
            );
            var lancamentoId = existingLancamento.Id;

            existingLancamento.ClearDomainEvents();
            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);
            _mockLancamentoRepository.UnitOfWork().Returns(1);

            var command = new DeleteLancamentoCommand(correlationId)
            {
                Id = lancamentoId,
            };

            var publishedEvents = new List<IDomainEvent>();
            await _mockDomainEventPublisher.Publish(Arg.Do<IEnumerable<IDomainEvent>>(events => publishedEvents.AddRange(events)));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            // [Requirement: HU-005] Verifica se o método de busca foi chamado
            await _mockLancamentoRepository.Received(1).GetByIdAsync(lancamentoId);
            await _mockLancamentoRepository.Received(1).DeleteAsync(lancamentoId);

            // [Requirement: RN-007] Verifica se UnitOfWork foi chamado para persistir a exclusão lógica
            await _mockLancamentoRepository.Received(1).UnitOfWork();

            // [Requirement: HU-005, NFR-OBS-001, NFR-SEG-005] Verifica se o evento de domínio foi publicado corretamente
            publishedEvents.Should().ContainSingle()
                .And.AllBeOfType<LancamentoExcluidoEvent>();

            var deletedEvent = publishedEvents.First() as LancamentoExcluidoEvent;

            deletedEvent.Should().NotBeNull();
            deletedEvent!.LancamentoId.Should().Be(lancamentoId);
            deletedEvent.Valor.Should().Be(existingLancamento.Valor);
            deletedEvent.Tipo.Should().Be(existingLancamento.Tipo.Valor);
            deletedEvent.Data.Should().Be(existingLancamento.Data.Date);
            deletedEvent.CorrelationId.Should().Be(command.CorrelationId);
            deletedEvent.OcurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact(DisplayName = "DeleteLancamento_Should_Return_False_If_Lancamento_Not_Found")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "HU-005, NFR-OBS-001, TDD")]
        public async Task Handle_DeleteLancamentoCommand_ShouldReturnFalseIfLancamentoNotFound()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var correlationId = Guid.NewGuid().ToString();
            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns((Lancamento)null);
            var command = new DeleteLancamentoCommand(correlationId)
            {
                Id = lancamentoId
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            await _mockLancamentoRepository.Received(1).GetByIdAsync(lancamentoId);
            await _mockLancamentoRepository.DidNotReceive().UpdateAsync(Arg.Any<Lancamento>()); // Não deve tentar atualizar/deletar
            await _mockLancamentoRepository.DidNotReceive().UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());

            // [Requirement: NFR-OBS-001] Verifica se o logger registrou o aviso com o CorrelationId
            var message = $"Lancamento with ID {lancamentoId} not found for deletion. CorrelationId: {command.CorrelationId}";
            _mockLogger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Select(callArguments => callArguments[2].ToString()).Should().Contain(message);
        }

        [Fact(DisplayName = "DeleteLancamento_Should_Not_Publish_Event_If_No_Rows_Affected")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "NFR-OBS-001, TDD")] // Adicionado NFR-OBS-001
        public async Task Handle_DeleteLancamentoCommand_ShouldNotPublishEventIfNoRowsAffected()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var correlationId = Guid.NewGuid().ToString();
            var existingLancamento = Lancamento.Create(100m, DateTime.Today, "A Excluir", "Debito", "Lazer", "Lançamento de teste", "initial-corr-id");
            existingLancamento.ClearDomainEvents();

            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);
            _mockLancamentoRepository.UnitOfWork().Returns(0); // Simula que 0 linhas foram afetadas (falha na persistência)

            var command = new DeleteLancamentoCommand(correlationId)
            {
                Id = lancamentoId,
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            await _mockLancamentoRepository.Received(1).GetByIdAsync(lancamentoId);
            await _mockLancamentoRepository.Received(1).UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>()); // Evento NÃO deve ser publicado

            // [Requirement: NFR-OBS-001] Verifica se o logger registrou a informação com o CorrelationId
            var message = $"Lancamento with ID {lancamentoId} deleted successfully (Rows affected: 0). CorrelationId: {command.CorrelationId}";
            _mockLogger.ReceivedCalls()
                            .Select(call => call.GetArguments())
                            .Select(callArguments => callArguments[2].ToString()).Should().Contain(message);
        }

        [Fact(DisplayName = "DeleteLancamento_Should_Throw_Exception_If_Deletion_Date_Is_Not_Today")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "HU-005, RN-001, TDD")] // HU-005 (critério de aceitação), RN-001
        public async Task Handle_DeleteLancamentoCommand_ShouldThrowExceptionIfDeletionDateIsNotToday()
        {
            // Arrange
            var lancamentoId = Guid.NewGuid();
            var correlationId = Guid.NewGuid().ToString();

            var existingLancamento = Lancamento.Create(
                100m,
                DateTime.Today.AddDays(-1),
                "Lançamento Antigo",
                "Debito",
                "Compras",
                "Lançamento de teste",
                "old-corr-id"
            );
            existingLancamento.ClearDomainEvents();

            _mockLancamentoRepository.GetByIdAsync(lancamentoId).Returns(existingLancamento);

            var command = new DeleteLancamentoCommand(correlationId)
            {
                Id = lancamentoId,
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            // [Requirement: RN-001] A entidade deve lançar a exceção de domínio.
            await act.Should().ThrowAsync<LancamentoDomainException>()
                .WithMessage("Lançamento só pode ser excluído se a data for igual ao dia atual.");

            await _mockLancamentoRepository.Received(1).GetByIdAsync(lancamentoId);
            await _mockLancamentoRepository.DidNotReceive().UpdateAsync(Arg.Any<Lancamento>());
            await _mockLancamentoRepository.DidNotReceive().UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());
        }
    }
}