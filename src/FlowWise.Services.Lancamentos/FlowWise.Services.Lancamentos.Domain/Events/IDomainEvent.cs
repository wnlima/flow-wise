using MediatR;

namespace FlowWise.Services.Lancamentos.Domain.Events
{
    /// <summary>
    /// [DDD] Abstração: Interface marcadora para eventos de domínio.
    /// Define um contrato básico para eventos que representam algo significativo
    /// que aconteceu no domínio e que outras partes da aplicação podem se interessar.
    /// Herda de <see cref="INotification"/> do MediatR.
    /// </summary>
    /// <remarks>
    /// [NFR-OBS-001]: Esta interface, em conjunto com DomainEventBase, garante que todos os eventos
    /// de domínio suportem rastreabilidade e observabilidade via Correlation ID e carimbo de data/hora.
    /// </remarks>
    public interface IDomainEvent : INotification
    {
        /// <summary>
        /// Um identificador único para o evento de domínio.
        /// </summary>
        Guid EventId { get; set; }

        /// <summary>
        /// A data e hora (UTC) em que o evento de domínio ocorreu.
        /// </summary>
        DateTime OcurredOn { get; set; }

        /// <summary>
        /// Um identificador único para rastrear a requisição que originou o evento.
        /// Essencial para observabilidade em sistemas distribuídos.
        /// </summary>
        string CorrelationId { get; set; }
    }
}