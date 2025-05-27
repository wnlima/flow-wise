using MassTransit;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Consolidacao.Domain.Exceptions;

namespace FlowWise.Services.Consolidacao.Application.EventConsumers;

/// <summary>
/// Consumidor de eventos para <see cref="LancamentoAtualizadoEvent"/>.
/// Este consumidor é responsável por atualizar o saldo diário consolidado
/// quando um lançamento existente é modificado no serviço de Lançamentos.
/// </summary>
/// <remarks>
/// [CQRS]: Parte da camada de leitura/projeção para o serviço de Consolidação.
/// [EventSourcing]: Reage a eventos de domínio publicados por outro microsserviço.
/// [HU-006]: Essencial para a atualização do saldo diário.
/// [NFR-RES-001]: Garante que o serviço de consolidação reage a lançamentos atualizados.
/// [NFR-OBS-001]: Logs detalhados para observabilidade.
/// [DDD]: Interage com a entidade de domínio SaldoDiario e seu repositório.
/// </remarks>
public class LancamentoAtualizadoEventConsumer : IConsumer<LancamentoAtualizadoEvent>
{
    private readonly ISaldoDiarioRepository _saldoDiarioRepository;
    private readonly ILogger<LancamentoAtualizadoEventConsumer> _logger;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="LancamentoAtualizadoEventConsumer"/>.
    /// </summary>
    /// <param name="saldoDiarioRepository">O repositório para operações de persistência de <see cref="SaldoDiario"/>.</param>
    /// <param name="logger">O logger para registrar informações.</param>
    public LancamentoAtualizadoEventConsumer(ISaldoDiarioRepository saldoDiarioRepository, ILogger<LancamentoAtualizadoEventConsumer> logger)
    {
        _saldoDiarioRepository = saldoDiarioRepository;
        _logger = logger;
    }

    /// <summary>
    /// Consome o evento <see cref="LancamentoAtualizadoEvent"/> e atualiza o saldo diário.
    /// </summary>
    /// <param name="context">O contexto do consumidor contendo a mensagem do evento.</param>
    /// <returns>Uma <see cref="Task"/> que representa a operação assíncrona.</returns>
    public async Task Consume(ConsumeContext<LancamentoAtualizadoEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing LancamentoAtualizadoEvent for LancamentoId: {LancamentoId}", message.LancamentoId);

        try
        {
            // Reverter o lançamento antigo (se ele mudou de data ou valor/tipo)
            // A comparação deve focar nas propriedades relevantes para o saldo (Data, Valor, Tipo).
            // Se as propriedades Data, Valor e Tipo não foram alteradas, não há impacto no saldo.
            if (message?.LancamentoAntigo.Data.Date != message?.LancamentoNovo.Data.Date ||
                message?.LancamentoAntigo.Valor != message?.LancamentoNovo.Valor ||
                message?.LancamentoAntigo.Tipo != message?.LancamentoNovo.Tipo)
            {
                // Lógica para reverter o lançamento antigo (se ele mudou de data ou valor/tipo)
                // Esta lógica ainda é necessária porque a mudança de data significa lidar com DOIS saldos diários.

                // Trata a reversão do lançamento na data antiga
                var oldDataLancamento = message.LancamentoAntigo.Data.Date; // Usar message.LancamentoAntigo.Data.Date
                var oldSaldoDiario = await _saldoDiarioRepository.GetByDateAsync(oldDataLancamento);

                if (oldSaldoDiario != null)
                {
                    oldSaldoDiario.ReverterLancamento(message.LancamentoAntigo.Tipo, message.LancamentoAntigo.Valor);
                    await _saldoDiarioRepository.UpdateAsync(oldSaldoDiario);
                    _logger.LogInformation("Reverted old transaction details from SaldoDiario for date {OldData}. Saldo Total: {OldSaldoTotal}. CorrelationId: {CorrelationId}",
                        oldDataLancamento.ToShortDateString(), oldSaldoDiario.SaldoTotal, message.CorrelationId);
                }
                else
                {
                    _logger.LogWarning("Old SaldoDiario for date {OldData} not found during update processing. This might indicate an inconsistency. CorrelationId: {CorrelationId}",
                        oldDataLancamento.ToShortDateString(), message.CorrelationId);
                }

                var newDataLancamento = message.LancamentoNovo.Data.Date;
                var newSaldoDiario = await _saldoDiarioRepository.GetByDateAsync(newDataLancamento);

                if (newSaldoDiario == null)
                {
                    newSaldoDiario = SaldoDiario.Create(newDataLancamento);
                    await _saldoDiarioRepository.AddAsync(newSaldoDiario);
                    _logger.LogInformation("New SaldoDiario created for date {NewData} due to updated transaction. CorrelationId: {CorrelationId}", newDataLancamento.ToShortDateString(), message.CorrelationId);
                }

                newSaldoDiario.AplicarLancamento(message.LancamentoNovo.Tipo, message.LancamentoNovo.Valor);
                await _saldoDiarioRepository.UpdateAsync(newSaldoDiario);
                _logger.LogInformation("Applied new transaction details to SaldoDiario for date {NewData}. Saldo Total: {NewSaldoTotal}. CorrelationId: {CorrelationId}",
                    newDataLancamento.ToShortDateString(), newSaldoDiario.SaldoTotal, message.CorrelationId);

                await _saldoDiarioRepository.UnitOfWork();
            }


            _logger.LogInformation("Finished processing LancamentoAtualizadoEvent for LancamentoId: {LancamentoId}. CorrelationId: {CorrelationId}",
                message.LancamentoId, message.CorrelationId);
        }
        catch (ConsolidacaoDomainException ex)
        {
            _logger.LogError(ex, "Domain validation error while processing LancamentoAtualizadoEvent for LancamentoId: {LancamentoId}. Message: {ErrorMessage}. CorrelationId: {CorrelationId}",
                message.LancamentoId, ex.Message, message.CorrelationId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing LancamentoAtualizadoEvent for LancamentoId: {LancamentoId}. CorrelationId: {CorrelationId}",
                message.LancamentoId, message.CorrelationId);
            throw;
        }
    }
}
