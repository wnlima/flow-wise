using FluentAssertions;
using NSubstitute;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.CommandHandlers;
using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Events;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.CommandHandlers
{
    public class CreateLancamentoCommandHandlerTests
    {
        private readonly ILancamentoRepository _mockLancamentoRepository;
        private readonly IDomainEventPublisher _mockDomainEventPublisher;
        private readonly ILogger<CreateLancamentoCommandHandler> _mockLogger;
        private readonly CreateLancamentoCommandHandler _handler;

        public CreateLancamentoCommandHandlerTests()
        {
            _mockLancamentoRepository = Substitute.For<ILancamentoRepository>();
            _mockDomainEventPublisher = Substitute.For<IDomainEventPublisher>();
            _mockLogger = Substitute.For<ILogger<CreateLancamentoCommandHandler>>();

            _handler = new CreateLancamentoCommandHandler(_mockLancamentoRepository, _mockDomainEventPublisher, _mockLogger);
        }

        [Fact(DisplayName = "CreateLancamento_Should_Create_And_Publish_Event_On_Success")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "HU-001, HU-002, NFR-OBS-001, TDD")]
        public async Task Handle_CreateLancamentoCommand_ShouldCreateLancamentoAndPublishEvent()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 250.75m,
                Data = DateTime.Today.AddDays(-1),
                Descricao = "Recebimento de Cliente X",
                Tipo = "Credito",
                Categoria = CategoriaLancamento.Salario.Valor,
                Observacoes = "Pagamento da fatura 123.",
            };

            Lancamento capturedLancamento = null;
            _mockLancamentoRepository.AddAsync(Arg.Do<Lancamento>(l => capturedLancamento = l))
                                     .Returns(Task.CompletedTask);

            _mockLancamentoRepository.UnitOfWork().Returns(1);

            // Capturar os eventos publicados
            var publishedEvents = new List<IDomainEvent>();
            await _mockDomainEventPublisher.Publish(Arg.Do<IEnumerable<IDomainEvent>>(events => publishedEvents.AddRange(events)));

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().NotBeEmpty();
            capturedLancamento.Should().NotBeNull();
            resultId.Should().Be(capturedLancamento.Id); // Verifica se o ID retornado é o mesmo da entidade criada

            await _mockLancamentoRepository.Received(1).AddAsync(Arg.Is<Lancamento>(l =>
                l.Valor == command.Valor &&
                l.Data.Date == command.Data.Date &&
                l.Descricao == command.Descricao &&
                l.Tipo.Valor == command.Tipo &&
                l.Categoria.Valor == command.Categoria &&
                l.Observacoes == command.Observacoes &&
                l.DataCriacao >= DateTime.UtcNow.AddSeconds(-5))
            );

            // [Requirement: TDD] Verifica se UnitOfWork foi chamado para persistir a entidade
            await _mockLancamentoRepository.Received(1).UnitOfWork();

            // [Requirement: HU-001, HU-002, NFR-OBS-001] Verifica se o evento de domínio foi publicado corretamente
            publishedEvents.Should().ContainSingle()
                .And.AllBeOfType<LancamentoRegistradoEvent>();

            var registeredEvent = publishedEvents.First() as LancamentoRegistradoEvent;

            registeredEvent.Should().NotBeNull();
            registeredEvent!.LancamentoId.Should().Be(resultId);
            registeredEvent.Valor.Should().Be(command.Valor);
            registeredEvent.Tipo.Should().Be(command.Tipo);
            registeredEvent.Data.Should().Be(command.Data.Date);
            registeredEvent.CorrelationId.Should().Be(command.CorrelationId);
            registeredEvent.OcurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact(DisplayName = "CreateLancamento_Should_Handle_DomainException_During_Creation")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")] // RN-006: Valor maior que zero. TDD: Teste de exceção.
        public async Task Handle_CreateLancamentoCommand_ShouldHandleDomainException()
        {
            // Arrange
            var invalidCommand = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = -10m, 
                Data = DateTime.Today,
                Descricao = "Teste de Lançamento Inválido",
                Tipo = "Credito",
                Categoria = "Teste",
            };

            // Act
            Func<Task> act = async () => await _handler.Handle(invalidCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LancamentoDomainException>()
                .WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");


            await _mockLancamentoRepository.DidNotReceive().AddAsync(Arg.Any<Lancamento>());
            await _mockLancamentoRepository.DidNotReceive().UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());
        }

        [Fact(DisplayName = "CreateLancamento_Should_Return_Empty_Guid_If_No_Rows_Affected")]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD")] // Este teste garante o comportamento em caso de falha de persistência
        public async Task Handle_CreateLancamentoCommand_ShouldReturnEmptyGuidIfNoRowsAffected()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 100.00m,
                Data = DateTime.Today,
                Descricao = "Teste de Falha de Persistência",
                Tipo = "Debito",
                Categoria = CategoriaLancamento.Alimentacao.ToString(),
                Observacoes = null,
            };

            Lancamento capturedLancamento = null;
            _mockLancamentoRepository.AddAsync(Arg.Do<Lancamento>(l => capturedLancamento = l))
                                     .Returns(Task.CompletedTask);

            _mockLancamentoRepository.UnitOfWork().Returns(0);

            // Act
            var resultId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().BeEmpty(); 

            await _mockLancamentoRepository.Received(1).AddAsync(Arg.Any<Lancamento>());
            await _mockLancamentoRepository.Received(1).UnitOfWork();
            await _mockDomainEventPublisher.DidNotReceive().Publish(Arg.Any<IEnumerable<IDomainEvent>>());
        }
    }
}