using FlowWise.Common.Events;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;

namespace FlowWise.Services.Lancamentos.Domain.Events
{
    /// <summary>
    /// Evento de domínio disparado quando um lançamento de caixa existente é atualizado.
    /// Este evento contém os dados do lançamento antes e depois da atualização,
    /// juntamente com o Correlation ID para rastreabilidade.
    /// </summary>
    /// <remarks>
    /// [HU-004]: A atualização de um lançamento gera este evento, que é crucial para a consistência eventual.
    /// [NFR-OBS-001]: O CorrelationId e OcurredOn (herdado de DomainEventBase) garantem a rastreabilidade.
    /// [NFR-RES-004]: Fornece o estado antigo e novo para permitir a compensação em transações distribuídas (Saga).
    /// </remarks>
    public class LancamentoAtualizadoEvent : EventBase, IDomainEvent
    {
        /// <summary>
        /// O identificador único do lançamento.
        /// </summary>
        public Guid LancamentoId { get; set; }
        /// <summary>
        /// Uma "fotografia" do lançamento antes da atualização, incluindo seus valores originais.
        /// Essencial para a lógica de compensação no serviço de consolidação.
        /// </summary>
        public LancamentoSnapshot LancamentoAntigo { get; set; }

        /// <summary>
        /// Uma "fotografia" do lançamento após a atualização, incluindo seus novos valores.
        /// Contém o estado final do lançamento após a operação.
        /// </summary>
        public LancamentoSnapshot LancamentoNovo { get; set; }

        /// <summary>
        /// Inicializa uma nova instância do evento <see cref="LancamentoAtualizadoEvent"/>.
        /// </summary>
        /// <param name="lancamentoAntigo">A fotografia do lançamento antes da atualização.</param>
        /// <param name="lancamentoNovo">A fotografia do lançamento após a atualização.</param>
        /// <param name="correlationId">O Correlation ID da operação. [NFR-OBS-001]</param>
        public LancamentoAtualizadoEvent(Guid id, LancamentoSnapshot lancamentoAntigo, LancamentoSnapshot lancamentoNovo, string correlationId)
            : base(correlationId)
        {
            LancamentoId = id;
            LancamentoAntigo = lancamentoAntigo;
            LancamentoNovo = lancamentoNovo;
        }
        
        public LancamentoAtualizadoEvent() : base(Guid.NewGuid().ToString()) { }
    }
}