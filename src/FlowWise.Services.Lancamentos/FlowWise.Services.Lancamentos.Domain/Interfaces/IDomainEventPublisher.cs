using FlowWise.Services.Lancamentos.Domain.Events;

namespace FlowWise.Services.Lancamentos.Domain.Interfaces;

/// <summary>
/// [DDD] Abstração: Interface para a publicação de eventos de domínio.
/// O domínio define que eventos podem ser publicados, mas não como são publicados.
/// A implementação concreta (e.g., via MediatR, MassTransit) é feita na camada de infraestrutura.
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publica um evento de domínio.
    /// </summary>
    /// <param name="domainEvent">O evento de domínio a ser publicado.</param>
    /// <returns>Uma Task que representa a operação assíncrona.</returns>
    Task Publish(IDomainEvent domainEvent, CancellationToken? cancellationToken = null);
    /// <summary>
    /// Publica uma coleção de eventos de domínio.
    /// </summary>
    /// <param name="domainEvents">A coleção de eventos de domínio a serem publicados.</param>
    /// <returns>Uma Task que representa a operação assíncrona.</returns>
    Task Publish(IEnumerable<IDomainEvent> domainEvents, CancellationToken? cancellationToken = null);
}
