using MassTransit;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Consolidacao.Domain.Exceptions;

namespace FlowWise.Services.Consolidacao.Application.EventConsumers
{
    /// <summary>
    /// Consumidor de eventos para <see cref="LancamentoExcluidoEvent"/>.
    /// Este consumidor é responsável por reverter o impacto de um lançamento excluído
    /// no saldo diário consolidado.
    /// </summary>
    /// <remarks>
    /// [CQRS]: Parte da camada de leitura/projeção para o serviço de Consolidação.
    /// [EventSourcing]: Reage a eventos de domínio publicados por outro microsserviço.
    /// [HU-006]: Essencial para manter a consistência do saldo diário após exclusões.
    /// [NFR-RES-001]: Garante que o serviço de consolidação reage a lançamentos excluídos.
    /// [NFR-OBS-001]: Logs detalhados para observabilidade.
    /// [DDD]: Interage com a entidade de domínio SaldoDiario e seu repositório.
    /// </remarks>
    public class LancamentoExcluidoEventConsumer : IConsumer<LancamentoExcluidoEvent>
    {
        private readonly ISaldoDiarioRepository _saldoDiarioRepository;
        private readonly ILogger<LancamentoExcluidoEventConsumer> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="LancamentoExcluidoEventConsumer"/>.
        /// </summary>
        /// <param name="saldoDiarioRepository">O repositório para operações de persistência de <see cref="SaldoDiario"/>.</param>
        /// <param name="logger">O logger para registrar informações.</param>
        public LancamentoExcluidoEventConsumer(ISaldoDiarioRepository saldoDiarioRepository, ILogger<LancamentoExcluidoEventConsumer> logger)
        {
            _saldoDiarioRepository = saldoDiarioRepository;
            _logger = logger;
        }

        /// <summary>
        /// Consome o evento <see cref="LancamentoExcluidoEvent"/> e reverte o lançamento no saldo diário.
        /// </summary>
        /// <param name="context">O contexto do consumidor contendo a mensagem do evento.</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona.</returns>
        public async Task Consume(ConsumeContext<LancamentoExcluidoEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing LancamentoExcluidoEvent for LancamentoId: {LancamentoId}, Data: {Data}, Valor: {Valor}, Tipo: {Tipo}, CorrelationId: {CorrelationId}",
             message.LancamentoId, message.Data, message.Valor, message.Tipo, message.CorrelationId);

            try
            {
                var dataLancamento = message.Data.Date;
                var saldoDiario = await _saldoDiarioRepository.GetByDateAsync(dataLancamento);

                if (saldoDiario == null)
                {
                    _logger.LogWarning("SaldoDiario for date {Data} not found for deleted LancamentoId: {LancamentoId}. This might indicate an inconsistency or that the balance was already zero. CorrelationId: {CorrelationId}",
                        dataLancamento.ToShortDateString(), message.LancamentoId, message.CorrelationId);
                    // Em um cenário de Event Sourcing puro, poderíamos recriar o SaldoDiario se necessário,
                    // mas para o POC, se não há saldo, significa que não há o que reverter.
                    return;
                }

                // Reverte o lançamento da entidade de domínio SaldoDiario
                // A entidade SaldoDiario é responsável por sua própria lógica de negócio
                saldoDiario.ReverterLancamento(message.Tipo, message.Valor);

                await _saldoDiarioRepository.UpdateAsync(saldoDiario);
                await _saldoDiarioRepository.UnitOfWork();

                _logger.LogInformation("SaldoDiario for date {Data} updated successfully after deletion of LancamentoId: {LancamentoId}. Saldo Total: {SaldoTotal}. CorrelationId: {CorrelationId}",
                    dataLancamento.ToShortDateString(), message.LancamentoId, saldoDiario.SaldoTotal, message.CorrelationId);
            }
            catch (ConsolidacaoDomainException ex)
            {
                _logger.LogError(ex, "Domain validation error while processing LancamentoExcluidoEvent for LancamentoId: {LancamentoId}. Message: {ErrorMessage}. CorrelationId: {CorrelationId}",
                    message.LancamentoId, ex.Message, message.CorrelationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing LancamentoExcluidoEvent for LancamentoId: {LancamentoId}. CorrelationId: {CorrelationId}",
                    message.LancamentoId, message.CorrelationId);
                throw;
            }
        }
    }
}