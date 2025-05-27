using FlowWise.Common.Models;
using FlowWise.Services.Lancamentos.Application.Dtos;

namespace FlowWise.Services.Lancamentos.Application.Queries
{
    /// <summary>
    /// Representa uma query para obter um lançamento específico pelo seu ID.
    /// Atende ao requisito de consultar um lançamento específico.
    /// </summary>
    /// <remarks>
    /// REQ-FLW-LAN-004: O sistema deve permitir a consulta de um lançamento específico.
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public class GetLancamentoByIdQuery : BaseQuery<LancamentoDto?>
    {
        /// <summary>
        /// O identificador único do lançamento a ser consultado.
        /// </summary>
        public Guid Id { get; private set; } // Tornar private set para imutabilidade após criação

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetLancamentoByIdQuery"/>.
        /// </summary>
        /// <param name="id">O identificador único do lançamento.</param>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade. Se nulo ou vazio, um novo GUID será gerado.</param>
        public GetLancamentoByIdQuery(Guid id, string? correlationId = null)
            : base(correlationId ?? Guid.NewGuid().ToString())
        {
            Id = id;
        }
    }
}