using MediatR;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Application.CommandHandlers
{
    /// <summary>
    /// [CQRS] Command Handler: Lida com o comando <see cref="DeleteLancamentoCommand"/>.
    /// Orquestra a exclusão de um lançamento existente, notificando outros serviços via evento de domínio.
    /// </summary>
    /// <remarks>
    /// [HU-003]: Responde pela funcionalidade de excluir lançamentos.
    /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado.
    /// [DDD]: Interage com o repositório e o publicador de eventos de domínio.
    /// </remarks>
    public class DeleteLancamentoCommandHandler : IRequestHandler<DeleteLancamentoCommand, bool>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly ILogger<DeleteLancamentoCommandHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="DeleteLancamentoCommandHandler"/>.
        /// </summary>
        /// <param name="lancamentoRepository">O repositório de lançamentos.</param>
        /// <param name="domainEventPublisher">O publicador de eventos de domínio.</param>
        /// <param name="logger">O logger.</param>
        public DeleteLancamentoCommandHandler(
            ILancamentoRepository lancamentoRepository,
            IDomainEventPublisher domainEventPublisher,
            ILogger<DeleteLancamentoCommandHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
        }

        /// <summary>
        /// Lida com a exclusão de um lançamento.
        /// </summary>
        /// <param name="request">O comando <see cref="DeleteLancamentoCommand"/>.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>True se o lançamento foi excluído; caso contrário, false.</returns>
        public async Task<bool> Handle(DeleteLancamentoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteLancamentoCommand for ID {LancamentoId} with CorrelationId: {CorrelationId}",
                request.Id, request.CorrelationId);

            var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id);

            if (lancamento == null)
            {
                _logger.LogWarning("Lancamento with ID {LancamentoId} not found for deletion. CorrelationId: {CorrelationId}",
                    request.Id, request.CorrelationId);
                return false; // Não lança exceção, apenas retorna false se não encontrar
            }

            lancamento.Delete(request.CorrelationId);

            await _lancamentoRepository.DeleteAsync(lancamento.Id); 
            var rowsAffected = await _lancamentoRepository.UnitOfWork(); 

            if (rowsAffected > 0)
            {
                await _domainEventPublisher.Publish(lancamento.DomainEvents);
                lancamento.ClearDomainEvents(); 
            }

            _logger.LogInformation("Lancamento with ID {LancamentoId} deleted successfully (Rows affected: {RowsAffected}). CorrelationId: {CorrelationId}",
                request.Id, rowsAffected, request.CorrelationId);

            return rowsAffected > 0;
        }
    }
}