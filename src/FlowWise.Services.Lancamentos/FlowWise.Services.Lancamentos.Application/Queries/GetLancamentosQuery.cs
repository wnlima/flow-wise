using FlowWise.Common.Models;
using FlowWise.Services.Lancamentos.Application.Dtos;

namespace FlowWise.Services.Lancamentos.Application.Queries
{
    /// <summary>
    /// Representa uma query para obter uma lista de lançamentos, com opções de filtro.
    /// Atende ao requisito de listar lançamentos.
    /// </summary>
    /// <remarks>
    /// REQ-FLW-LAN-001: O sistema deve permitir o registro de lançamentos (entrada e saída) na data atual.
    /// HU-001: Como usuário do sistema, quero registrar, listar e consultar lançamentos financeiros, para ter controle do meu fluxo de caixa diário.
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public class GetLancamentosQuery : BaseQuery<IEnumerable<LancamentoDto>>
    {
        /// <summary>
        /// Data de início para o filtro de lançamentos (formato YYYY-MM-DD).
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// Data de fim para o filtro de lançamentos (formato YYYY-MM-DD).
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Tipo de lançamento para o filtro ("Credito" ou "Debito").
        /// </summary>
        public string? Tipo { get; private set; }

        /// <summary>
        /// Categoria do lançamento para o filtro.
        /// </summary>
        public string? Categoria { get; private set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetLancamentosQuery"/> com filtros.
        /// </summary>
        /// <param name="startDate">Data de início para o filtro (opcional).</param>
        /// <param name="endDate">Data de fim para o filtro (opcional).</param>
        /// <param name="tipo">Tipo de lançamento para o filtro (opcional).</param>
        /// <param name="categoria">Categoria do lançamento para o filtro (opcional).</param>
        /// <param name="correlationId">O identificador de correlação para rastreabilidade. Se nulo ou vazio, um novo GUID será gerado.</param>
        public GetLancamentosQuery(DateTime? startDate, DateTime? endDate, string? tipo, string? categoria, string? correlationId = null)
            : base(correlationId ?? Guid.NewGuid().ToString())
        {
            StartDate = startDate;
            EndDate = endDate;
            Tipo = tipo;
            Categoria = categoria;
        }
    }
}