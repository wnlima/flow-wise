using FlowWise.Common.Models;
using FlowWise.Services.Consolidacao.Application.Dtos;

namespace FlowWise.Services.Consolidacao.Application.Queries
{
    /// <summary>
    /// [CQRS] Query: Representa a requisição para obter um relatório de fluxo de caixa para um período.
    /// Herda de <see cref="BaseQuery{TResponse}"/> para inclusão do CorrelationId.
    /// </summary>
    /// <remarks>
    /// [HU-007]: Permite a geração de relatório de fluxo de caixa por período.
    /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado para rastreabilidade.
    /// </remarks>
    public class GetRelatorioFluxoCaixaQuery : BaseQuery<RelatorioFluxoCaixaDto>
    {
        /// <summary>
        /// A data de início do período do relatório (apenas a parte da data).
        /// </summary>
        public DateTime DataInicio { get; private set; }
        /// <summary>
        /// A data de fim do período do relatório (apenas a parte da data).
        /// </summary>
        public DateTime DataFim { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetRelatorioFluxoCaixaQuery"/>.
        /// </summary>
        /// <param name="dataInicio">A data de início do período.</param>
        /// <param name="dataFim">A data de fim do período.</param>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade. Se nulo ou vazio, um novo GUID será gerado.</param>
        public GetRelatorioFluxoCaixaQuery(DateTime dataInicio, DateTime dataFim, string? correlationId = null)
            : base(correlationId)
        {
            DataInicio = dataInicio.Date;
            DataFim = dataFim.Date;
        }
    }
}