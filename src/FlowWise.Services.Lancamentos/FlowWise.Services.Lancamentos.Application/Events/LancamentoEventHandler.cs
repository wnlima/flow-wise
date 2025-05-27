using MediatR;
using Microsoft.Extensions.Logging;
using FlowWise.Services.Lancamentos.Domain.Events;
using FlowWise.Services.Lancamentos.Domain.Interfaces;

namespace FlowWise.Services.Lancamentos.Application.Events;

public class LancamentoEventHandler : INotificationHandler<LancamentoAtualizadoEvent>, INotificationHandler<LancamentoRegistradoEvent>, INotificationHandler<LancamentoExcluidoEvent>
{
    private readonly ILancamentoEventPublisher _externalEventPublisher;
    private readonly ILogger<LancamentoEventHandler> _logger;

    public LancamentoEventHandler(ILancamentoEventPublisher externalEventPublisher, ILogger<LancamentoEventHandler> logger)
    {
        _externalEventPublisher = externalEventPublisher;
        _logger = logger;
    }

    public async Task Handle(LancamentoAtualizadoEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Novo evento interno para publicação externa: Id={notification.EventId}, Tipo={notification.GetType().Name}");

        await _externalEventPublisher.Publish(notification, cancellationToken);

        _logger.LogInformation($"Evento {notification.GetType().Name} publicado externamente com sucesso. Id={notification.EventId}");
    }

    public async Task Handle(LancamentoRegistradoEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Novo evento interno para publicação externa: Id={notification.EventId}, Tipo={notification.GetType().Name}");

        await _externalEventPublisher.Publish(notification, cancellationToken);

        _logger.LogInformation($"Evento {notification.GetType().Name} publicado externamente com sucesso. Id={notification.EventId}");
    }

    public async Task Handle(LancamentoExcluidoEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Novo evento interno para publicação externa: Id={notification.EventId}, Tipo={notification.GetType().Name}");

        await _externalEventPublisher.Publish(notification, cancellationToken);

        _logger.LogInformation($"Evento {notification.GetType().Name} publicado externamente com sucesso. Id={notification.EventId}");
    }
}
