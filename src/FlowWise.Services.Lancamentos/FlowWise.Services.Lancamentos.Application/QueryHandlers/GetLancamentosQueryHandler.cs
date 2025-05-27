using FlowWise.Services.Lancamentos.Application.Queries;
using FlowWise.Services.Lancamentos.Application.Dtos;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Application.QueryHandlers
{
    /// <summary>
    /// Manipulador para a query <see cref="GetLancamentosQuery"/>.
    /// Responsável por buscar uma lista de lançamentos, com opções de filtro.
    /// </summary>
    /// <remarks>
    /// Atende ao requisito REQ-FLW-LAN-001, HU-001 (listagem e consulta).
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public class GetLancamentosQueryHandler : IRequestHandler<GetLancamentosQuery, IEnumerable<LancamentoDto>>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly ILogger<GetLancamentosQueryHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetLancamentosQueryHandler"/>.
        /// </summary>
        /// <param name="lancamentoRepository">O repositório de lançamentos.</param>
        /// <param name="logger">O logger para registrar informações.</param>
        public GetLancamentosQueryHandler(ILancamentoRepository lancamentoRepository, ILogger<GetLancamentosQueryHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Lida com a requisição de busca de múltiplos lançamentos, aplicando filtros se fornecidos.
        /// </summary>
        /// <param name="request">A query <see cref="GetLancamentosQuery"/>.</param>
        /// <param name="cancellationToken">Um token para cancelar a operação.</param>
        /// <returns>Uma coleção de <see cref="LancamentoDto"/>.</returns>
        public async Task<IEnumerable<LancamentoDto>> Handle(GetLancamentosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetLancamentosQuery with CorrelationId: {CorrelationId}. Filters: StartDate={StartDate}, EndDate={EndDate}, Tipo={Tipo}, Categoria={Categoria}",
                request.CorrelationId, request.StartDate, request.EndDate, request.Tipo, request.Categoria);

            var lancamentos = await _lancamentoRepository.GetAllAsync(
                request.StartDate,
                request.EndDate,
                request.Tipo,
                request.Categoria
            );

            _logger.LogInformation("Found {Count} lancamentos for CorrelationId: {CorrelationId}", lancamentos.Count(), request.CorrelationId);

            return lancamentos.Select(lancamento => new LancamentoDto
            {
                Id = lancamento.Id,
                Valor = lancamento.Valor,
                Data = lancamento.Data,
                Descricao = lancamento.Descricao,
                Tipo = lancamento.Tipo.Valor,
                Categoria = lancamento.Categoria.Valor,
                Observacoes = lancamento.Observacoes,
                CorrelationId = request.CorrelationId
            }).ToList();
        }
    }
}