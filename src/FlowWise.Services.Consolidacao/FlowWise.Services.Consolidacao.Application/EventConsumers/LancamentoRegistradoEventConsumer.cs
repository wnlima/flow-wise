using MassTransit;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Consolidacao.Domain.Exceptions;

namespace FlowWise.Services.Consolidacao.Application.EventConsumers
{
    /// <summary>
    /// Consumidor de eventos para <see cref="LancamentoRegistradoEvent"/>.
    /// Este consumidor é responsável por atualizar o saldo diário consolidado
    /// quando um novo lançamento é registrado no serviço de Lançamentos.
    /// </summary>
    /// <remarks>
    /// [CQRS]: Parte da camada de leitura/projeção para o serviço de Consolidação.
    /// [EventSourcing]: Reage a eventos de domínio publicados por outro microsserviço.
    /// [HU-006]: Essencial para a atualização do saldo diário.
    /// [NFR-RES-001]: Garante que o serviço de consolidação reage a novos lançamentos.
    /// [NFR-OBS-001]: Logs detalhados para observabilidade.
    /// [DDD]: Interage com a entidade de domínio SaldoDiario e seu repositório.
    /// </remarks>
    public class LancamentoRegistradoEventConsumer : IConsumer<LancamentoRegistradoEvent>
    {
        private readonly ISaldoDiarioRepository _saldoDiarioRepository;
        private readonly ILogger<LancamentoRegistradoEventConsumer> _logger;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="LancamentoRegistradoEventConsumer"/>.
        /// </summary>
        /// <param name="saldoDiarioRepository">O repositório para operações de persistência de <see cref="SaldoDiario"/>.</param>
        /// <param name="logger">O logger para registrar informações.</param>
        public LancamentoRegistradoEventConsumer(ISaldoDiarioRepository saldoDiarioRepository, ILogger<LancamentoRegistradoEventConsumer> logger)
        {
            _saldoDiarioRepository = saldoDiarioRepository;
            _logger = logger;
        }

        /// <summary>
        /// Consome o evento <see cref="LancamentoRegistradoEvent"/> e atualiza o saldo diário.
        /// </summary>
        /// <param name="context">O contexto do consumidor contendo a mensagem do evento.</param>
        /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona.</returns>
        public async Task Consume(ConsumeContext<LancamentoRegistradoEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation("Processing LancamentoRegistradoEvent for LancamentoId: {LancamentoId}, Data: {Data}, Valor: {Valor}, Tipo: {Tipo}, CorrelationId: {CorrelationId}",
              message.LancamentoId, message.Data, message.Valor, message.Tipo, message.CorrelationId);

            try
            {
                var dataLancamento = message.Data.Date;
                var saldoDiario = await _saldoDiarioRepository.GetByDateAsync(dataLancamento);
                var isNew = false;

                if (saldoDiario == null)
                {
                    // Se não existe saldo para a data, cria um novo
                    saldoDiario = SaldoDiario.Create(dataLancamento);
                    isNew = true;
                }

                // Aplica o lançamento à entidade de domínio SaldoDiario
                // A entidade SaldoDiario é responsável por sua própria lógica de negócio
                saldoDiario.AplicarLancamento(message.Tipo, message.Valor);

                if (isNew)
                {
                    await _saldoDiarioRepository.AddAsync(saldoDiario);
                    _logger.LogInformation("New SaldoDiario created for date {Data}. CorrelationId: {CorrelationId}", dataLancamento.ToShortDateString(), message.CorrelationId);
                }
                else
                {
                    await _saldoDiarioRepository.UpdateAsync(saldoDiario);
                }

                await _saldoDiarioRepository.UnitOfWork();

                _logger.LogInformation("SaldoDiario for date {Data} updated successfully. Saldo Total: {SaldoTotal}. CorrelationId: {CorrelationId}",
                    dataLancamento.ToShortDateString(), saldoDiario.SaldoTotal, message.CorrelationId);
            }
            catch (ConsolidacaoDomainException ex)
            {
                _logger.LogError(ex, "Domain validation error while processing LancamentoRegistradoEvent for LancamentoId: {LancamentoId}. Message: {ErrorMessage}. CorrelationId: {CorrelationId}",
                    message.LancamentoId, ex.Message, message.CorrelationId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing LancamentoRegistradoEvent for LancamentoId: {LancamentoId}. CorrelationId: {CorrelationId}",
                    message.LancamentoId, message.CorrelationId);
                throw;
            }
        }
    }
}