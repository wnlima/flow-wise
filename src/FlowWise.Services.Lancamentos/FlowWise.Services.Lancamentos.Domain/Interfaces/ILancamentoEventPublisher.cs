using FlowWise.Services.Lancamentos.Domain.Events;

namespace FlowWise.Services.Lancamentos.Domain.Interfaces;

public interface ILancamentoEventPublisher
{
    Task Publish(LancamentoAtualizadoEvent @event, CancellationToken? cancellationToken = null);
    Task Publish(LancamentoRegistradoEvent @event, CancellationToken? cancellationToken = null);
    Task Publish(LancamentoExcluidoEvent @event, CancellationToken? cancellationToken = null);
}