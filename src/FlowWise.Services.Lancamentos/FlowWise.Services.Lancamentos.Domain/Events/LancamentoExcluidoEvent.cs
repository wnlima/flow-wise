using FlowWise.Common.Events;

namespace FlowWise.Services.Lancamentos.Domain.Events
{
    /// <summary>
    /// Evento de domínio disparado quando um lançamento de caixa é excluído.
    /// Contém os dados do lançamento excluído para permitir a reversão do impacto no consolidado.
    /// </summary>
    /// <remarks>
    /// [HU-003]: A exclusão de um lançamento gera este evento, que é crucial para a consistência eventual.
    /// [NFR-OBS-001]: O CorrelationId e OcurredOn (herdado de DomainEventBase) garantem a rastreabilidade.
    /// </remarks>
    public class LancamentoExcluidoEvent : EventBase, IDomainEvent
    {
        /// <summary>
        /// O identificador único do lançamento excluído.
        /// </summary>
        public Guid LancamentoId { get; set; }
        /// <summary>
        /// O valor do lançamento excluído.
        /// </summary>
        public decimal Valor { get; set; }
        /// <summary>
        /// O tipo do lançamento excluído (Crédito/Débito).
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// A data do lançamento excluído.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Inicializa uma nova instância do evento <see cref="LancamentoExcluidoEvent"/>.
        /// </summary>
        /// <param name="lancamentoId">O ID do lançamento.</param>
        /// <param name="valor">O valor do lançamento.</param>
        /// <param name="tipo">O tipo do lançamento.</param>
        /// <param name="data">A data do lançamento.</param>
        /// <param name="correlationId">O Correlation ID da operação. [NFR-OBS-001]</param>
        public LancamentoExcluidoEvent(Guid lancamentoId, decimal valor, string tipo, DateTime data, string correlationId)
            : base(correlationId)
        {
            LancamentoId = lancamentoId;
            Valor = valor;
            Tipo = tipo;
            Data = data;
        }

        public LancamentoExcluidoEvent() : base(Guid.NewGuid().ToString()) { }
    }
}