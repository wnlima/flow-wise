using FlowWise.Common.Events;

namespace FlowWise.Services.Lancamentos.Domain.Events
{
    /// <summary>
    /// Evento de domínio que é disparado quando um novo lançamento de caixa é registrado com sucesso.
    /// Este evento contém os dados essenciais do lançamento para que outros agregados ou serviços
    /// possam reagir a ele (e.g., o serviço de consolidação).
    /// </summary>
    /// <remarks>
    /// [HU-001]: A criação de um lançamento gera este evento, que é crucial para o CQRS e Event Sourcing.
    /// [NFR-OBS-001]: O CorrelationId e OcurredOn (herdado de DomainEventBase) garantem a rastreabilidade.
    /// </remarks>
    public class LancamentoRegistradoEvent : EventBase, IDomainEvent
    {
        /// <summary>
        /// O identificador único do lançamento.
        /// </summary>
        public Guid LancamentoId { get; set; }
        /// <summary>
        /// O valor do lançamento.
        /// </summary>
        public decimal Valor { get; set; }
        /// <summary>
        /// O tipo do lançamento (Crédito/Débito).
        /// </summary>
        public string Tipo { get; set; }
        /// <summary>
        /// A data do lançamento.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Inicializa uma nova instância do evento <see cref="LancamentoRegistradoEvent"/>.
        /// </summary>
        /// <param name="lancamentoId">O ID do lançamento.</param>
        /// <param name="valor">O valor do lançamento.</param>
        /// <param name="tipo">O tipo do lançamento.</param>
        /// <param name="data">A data do lançamento.</param>
        /// <param name="correlationId">O Correlation ID da operação. [NFR-OBS-001]</param>
        public LancamentoRegistradoEvent(Guid lancamentoId, decimal valor, string tipo, DateTime data, string correlationId)
            : base(correlationId)
        {
            LancamentoId = lancamentoId;
            Valor = valor;
            Tipo = tipo;
            Data = data;
        }

        public LancamentoRegistradoEvent() : base(Guid.NewGuid().ToString()) { }
    }
}