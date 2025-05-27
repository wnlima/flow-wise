using MediatR;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Domain.Interfaces;

namespace FlowWise.Services.Lancamentos.Application.Events;

public class DomainEventPublisher : IDomainEventPublisher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventPublisher> _logger;
    public DomainEventPublisher(IMediator mediator, ILogger<DomainEventPublisher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Publish(IDomainEvent @event, CancellationToken? cancellationToken = null)
    {
        await _mediator.Publish(@event, cancellationToken ?? CancellationToken.None);
        _logger.LogInformation($"Evento de domínio publicado: {nameof(@event)} com ID {@event.EventId} e tipo {@event.GetType().Name}");
    }

    public async Task Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken? cancellationToken = null)
    {
        await Parallel.ForEachAsync(domainEvents, cancellationToken ?? CancellationToken.None, async (domainEvent, c) =>
        {
            await Publish(domainEvent, c);
        });
    }
}
