namespace FlowWise.Services.Consolidacao.Api.Responses
{
    /// <summary>
    /// DTO (Data Transfer Object) para a resposta de consulta de saldo diário consolidado.
    /// Representa a projeção de saldo para um dia específico, otimizado para leitura.
    /// </summary>
    /// <remarks>
    /// [HU-006]: Utilizado para visualização do saldo diário consolidado (D-1), conforme especificado nos requisitos funcionais.
    /// [NFR-PERF-002]: Este DTO é parte da resposta do serviço de consolidação que deve suportar 50 requisições/segundo.
    /// </remarks>
    public class SaldoDiarioResponse
    {
        /// <summary>
        /// A data do dia consolidado a que o saldo se refere (apenas a parte da data).
        /// [RN-003]: Refere-se sempre ao dia anterior (D-1) em relação à data atual da consulta.
        /// </summary>
        /// <example>2025-05-22</example>
        public DateTime Data { get; set; }
        /// <summary>
        /// O saldo total líquido para o dia, calculado como TotalCréditos - TotalDébitos.
        /// </summary>
        /// <example>1250.75</example>
        public decimal SaldoTotal { get; set; }
        /// <summary>
        /// O valor total de créditos (entradas) para o dia.
        /// </summary>
        /// <example>2500.00</example>
        public decimal TotalCreditos { get; set; }
        /// <summary>
        /// O valor total de débitos (saídas) para o dia.
        /// </summary>
        /// <example>1249.25</example>
        public decimal TotalDebitos { get; set; }
        /// <summary>
        /// A data e hora (UTC) da última atualização deste registro de saldo.
        /// [NFR-SEG-005]: Propriedade para fins de auditoria e rastreabilidade.
        /// </summary>
        /// <example>2025-05-23T03:00:00Z</example>
        public DateTime? UltimaAtualizacao { get; set; }
    }
}