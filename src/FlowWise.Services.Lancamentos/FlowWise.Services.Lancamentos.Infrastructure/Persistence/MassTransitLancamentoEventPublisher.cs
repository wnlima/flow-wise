using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using MassTransit;

namespace FlowWise.Services.Lancamentos.Infrastructure.Persistence;

public class MassTransitLancamentoEventPublisher : ILancamentoEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    public MassTransitLancamentoEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task Publish(LancamentoAtualizadoEvent @event, CancellationToken? cancellationToken = null)
    {
        return _publishEndpoint.Publish(@event, cancellationToken ?? CancellationToken.None);
    }

    public Task Publish(LancamentoRegistradoEvent @event, CancellationToken? cancellationToken = null)
    {
        return _publishEndpoint.Publish(@event, cancellationToken ?? CancellationToken.None);
    }

    public Task Publish(LancamentoExcluidoEvent @event, CancellationToken? cancellationToken = null)
    {
        return _publishEndpoint.Publish(@event, cancellationToken ?? CancellationToken.None);
    }
}