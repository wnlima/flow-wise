using MediatR;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Application.CommandHandlers
{
    /// <summary>
    /// [CQRS] Command Handler: Lida com o comando <see cref="CreateLancamentoCommand"/>.
    /// Orquestra a criação de um novo lançamento, persistindo-o e publicando seus eventos de domínio.
    /// </summary>
    /// <remarks>
    /// [HU-001]: Responde pela funcionalidade de registrar novos lançamentos.
    /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado e usado no log e eventos.
    /// [DDD]: Interage com o repositório e o publicador de eventos de domínio.
    /// </remarks>
    public class CreateLancamentoCommandHandler : IRequestHandler<CreateLancamentoCommand, Guid>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IDomainEventPublisher _domainEventPublisher; // Para publicar eventos
        private readonly ILogger<CreateLancamentoCommandHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="CreateLancamentoCommandHandler"/>.
        /// </summary>
        /// <param name="lancamentoRepository">O repositório de lançamentos para persistência.</param>
        /// <param name="domainEventPublisher">O publicador de eventos de domínio.</param>
        /// <param name="logger">O logger para registrar informações.</param>
        public CreateLancamentoCommandHandler(
            ILancamentoRepository lancamentoRepository,
            IDomainEventPublisher domainEventPublisher,
            ILogger<CreateLancamentoCommandHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
        }

        /// <summary>
        /// Lida com a criação de um novo lançamento.
        /// </summary>
        /// <param name="request">O comando <see cref="CreateLancamentoCommand"/>.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>O ID do lançamento criado.</returns>
        public async Task<Guid> Handle(CreateLancamentoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateLancamentoCommand for value {Valor} and date {Data} with CorrelationId: {CorrelationId}",
                request.Valor, request.Data, request.CorrelationId);

            var lancamento = Lancamento.Create(
                request.Valor,
                request.Data,
                request.Descricao,
                request.Tipo,
                request.Categoria,
                request.Observacoes,
                request.CorrelationId
            );

            await _lancamentoRepository.AddAsync(lancamento);

            if (await _lancamentoRepository.UnitOfWork() == 0)
                return Guid.Empty;

            await _domainEventPublisher.Publish(lancamento.DomainEvents);

            lancamento.ClearDomainEvents();

            _logger.LogInformation("Lancamento with ID {LancamentoId} created successfully. CorrelationId: {CorrelationId}",
                lancamento.Id, request.CorrelationId);

            return lancamento.Id;
        }
    }
}