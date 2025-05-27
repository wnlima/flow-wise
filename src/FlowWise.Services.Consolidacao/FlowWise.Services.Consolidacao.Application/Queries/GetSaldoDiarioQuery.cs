using FlowWise.Common.Models;
using FlowWise.Services.Consolidacao.Application.Dtos;

namespace FlowWise.Services.Consolidacao.Application.Queries
{
    /// <summary>
    /// [CQRS] Query: Representa a requisição para obter o saldo diário consolidado para uma data específica.
    /// Herda de <see cref="BaseQuery{TResponse}"/> para inclusão do CorrelationId.
    /// </summary>
    /// <remarks>
    /// [HU-006]: Permite a visualização do saldo diário consolidado (D-1).
    /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado para rastreabilidade.
    /// </remarks>
    public class GetSaldoDiarioQuery : BaseQuery<SaldoDiarioDto?>
    {
        /// <summary>
        /// A data para a qual o saldo diário consolidado é solicitado.
        /// [RN-003]: O saldo é sempre de D-1 (dia anterior).
        /// </summary>
        public DateTime Data { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetSaldoDiarioQuery"/>.
        /// </summary>
        /// <param name="data">A data para a qual o saldo diário é solicitado.</param>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade. Se nulo ou vazio, um novo GUID será gerado.</param>
        public GetSaldoDiarioQuery(DateTime data, string? correlationId = null)
            : base(correlationId)
        {
            Data = data.Date; 
        }
    }
}