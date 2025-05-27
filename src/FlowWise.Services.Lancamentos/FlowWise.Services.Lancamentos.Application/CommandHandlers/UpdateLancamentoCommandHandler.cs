using MediatR;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using FlowWise.Services.Lancamentos.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Application.CommandHandlers
{
    /// <summary>
    /// [CQRS] Command Handler: Lida com o comando <see cref="UpdateLancamentoCommand"/>.
    /// Orquestra a atualização de um lançamento existente, persistindo-o e publicando seus eventos de domínio.
    /// </summary>
    /// <remarks>
    /// [HU-002]: Responde pela funcionalidade de atualizar lançamentos.
    /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado.
    /// [DDD]: Interage com o repositório e o publicador de eventos de domínio.
    /// </remarks>
    public class UpdateLancamentoCommandHandler : IRequestHandler<UpdateLancamentoCommand, bool>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly ILogger<UpdateLancamentoCommandHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="UpdateLancamentoCommandHandler"/>.
        /// </summary>
        /// <param name="lancamentoRepository">O repositório de lançamentos.</param>
        /// <param name="domainEventPublisher">O publicador de eventos de domínio.</param>
        /// <param name="logger">O logger.</param>
        public UpdateLancamentoCommandHandler(
            ILancamentoRepository lancamentoRepository,
            IDomainEventPublisher domainEventPublisher,
            ILogger<UpdateLancamentoCommandHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
        }

        /// <summary>
        /// Lida com a atualização de um lançamento.
        /// </summary>
        /// <param name="request">O comando <see cref="UpdateLancamentoCommand"/>.</param>
        /// <param name="cancellationToken">O token de cancelamento.</param>
        /// <returns>True se o lançamento foi atualizado; caso contrário, false.</returns>
        /// <exception cref="LancamentoDomainException">Lançada se o lançamento não for encontrado.</exception>
        public async Task<bool> Handle(UpdateLancamentoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateLancamentoCommand for ID {LancamentoId} with CorrelationId: {CorrelationId}",
                request.Id, request.CorrelationId);

            var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id);

            if (lancamento == null)
            {
                _logger.LogWarning("Lancamento with ID {LancamentoId} not found for update. CorrelationId: {CorrelationId}",
                    request.Id, request.CorrelationId);
                throw new LancamentoDomainException($"Lançamento com ID '{request.Id}' não encontrado.");
            }

            lancamento.Update(
                request.Valor,
                request.Data,
                request.Descricao,
                request.Tipo,
                request.Categoria,
                request.Observacoes,
                request.CorrelationId
            );

            await _lancamentoRepository.UpdateAsync(lancamento);
            var rowsAffected = await _lancamentoRepository.UnitOfWork(); 

            if (rowsAffected > 0)
            {
                await _domainEventPublisher.Publish(lancamento.DomainEvents);
                lancamento.ClearDomainEvents(); 
            }

            _logger.LogInformation("Lancamento with ID {LancamentoId} updated successfully (Rows affected: {RowsAffected}). CorrelationId: {CorrelationId}",
                lancamento.Id, rowsAffected, request.CorrelationId);

            return rowsAffected > 0;
        }
    }
}