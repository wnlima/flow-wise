using MediatR;

namespace FlowWise.Common.Events
{
    /// <summary>
    /// [DDD] Base Abstrata: Fornece uma implementação base para todos os eventos.
    /// Garante que todos os eventos tenham um carimbo de data/hora (OcurredOn), identificador de evento (EventId),
    /// e um CorrelationId para rastreabilidade em sistemas distribuídos.
    /// Implementa <see cref="INotification"/> para integração com MediatR.
    /// </summary>
    /// <remarks>
    /// [NFR-OBS-001]: O Correlation ID é essencial para a observabilidade e rastreabilidade de requisições.
    /// [NFR-SEG-005]: A propriedade OcurredOn contribui para a auditabilidade das operações.
    /// </remarks>
    public abstract class EventBase
    {
        /// <summary>
        /// Um identificador único para o evento de domínio.
        /// </summary>
        public Guid EventId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// A data e hora (UTC) em que o evento de domínio ocorreu.
        /// </summary>
        public DateTime OcurredOn { get; set; }

        /// <summary>
        /// Um identificador único para rastrear a requisição que originou o evento.
        /// Essencial para observabilidade em sistemas distribuídos.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// Construtor protegido para inicializar as propriedades base do evento de domínio.
        /// </summary>
        /// <param name="correlationId">O identificador de correlação da operação.</param>
        protected EventBase(string correlationId)
        {
            OcurredOn = DateTime.UtcNow;
            CorrelationId = correlationId;
        }
    }
}