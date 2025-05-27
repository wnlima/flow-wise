namespace FlowWise.Services.Consolidacao.Api.Responses
{
    /// <summary>
    /// DTO (Data Transfer Object) para a resposta de consulta de relatório de fluxo de caixa por período.
    /// Agrega informações financeiras para um intervalo de datas.
    /// </summary>
    /// <remarks>
    /// [HU-007]: Utilizado para gerar relatórios de fluxo de caixa por período, conforme especificado nos requisitos funcionais.
    /// </remarks>
    public class RelatorioFluxoCaixaResponse
    {
        /// <summary>
        /// A data de início do período do relatório (apenas a parte da data).
        /// </summary>
        /// <example>2025-05-01</example>
        public DateTime DataInicio { get; set; }
        /// <summary>
        /// A data de fim do período do relatório (apenas a parte da data).
        /// </summary>
        /// <example>2025-05-31</example>
        public DateTime DataFim { get; set; }
        /// <summary>
        /// O saldo de caixa no início do período do relatório.
        /// Este é o saldo final do dia anterior à <see cref="DataInicio"/>.
        /// </summary>
        /// <example>1500.25</example>
        public decimal SaldoInicial { get; set; }
        /// <summary>
        /// O valor total de créditos (entradas) acumulado no período do relatório.
        /// </summary>
        /// <example>5000.00</example>
        public decimal TotalCreditos { get; set; }
        /// <summary>
        /// O valor total de débitos (saídas) acumulado no período do relatório.
        /// </summary>
        /// <example>3000.75</example>
        public decimal TotalDebitos { get; set; }
        /// <summary>
        /// O saldo de caixa no final do período do relatório.
        /// Este é o saldo final calculado até o final da <see cref="DataFim"/>.
        /// </summary>
        /// <example>3499.50</example>
        public decimal SaldoFinal { get; set; }
    }
}