using FlowWise.Services.Lancamentos.Application.Queries;
using FlowWise.Services.Lancamentos.Application.Dtos;
using FlowWise.Services.Lancamentos.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Lancamentos.Application.QueryHandlers
{
    /// <summary>
    /// Manipulador para a query <see cref="GetLancamentoByIdQuery"/>.
    /// Responsável por buscar um lançamento específico pelo seu ID.
    /// </summary>
    /// <remarks>
    /// Atende ao requisito REQ-FLW-LAN-004 e HU-001 (consulta).
    /// NFR-OBS-001: O sistema deve registrar um Correlation ID para rastreabilidade de requisições.
    /// </remarks>
    public class GetLancamentoByIdQueryHandler : IRequestHandler<GetLancamentoByIdQuery, LancamentoDto?>
    {
        private readonly ILancamentoRepository _lancamentoRepository;
        private readonly ILogger<GetLancamentoByIdQueryHandler> _logger;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="GetLancamentoByIdQueryHandler"/>.
        /// </summary>
        /// <param name="lancamentoRepository">O repositório de lançamentos.</param>
        /// <param name="logger">O logger para registrar informações.</param>
        public GetLancamentoByIdQueryHandler(ILancamentoRepository lancamentoRepository, ILogger<GetLancamentoByIdQueryHandler> logger)
        {
            _lancamentoRepository = lancamentoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Lida com a requisição de busca de um lançamento por ID.
        /// </summary>
        /// <param name="request">A query <see cref="GetLancamentoByIdQuery"/>.</param>
        /// <param name="cancellationToken">Um token para cancelar a operação.</param>
        /// <returns>Um objeto <see cref="LancamentoDto"/> se encontrado, caso contrário, null.</returns>
        public async Task<LancamentoDto?> Handle(GetLancamentoByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetLancamentoByIdQuery for ID {LancamentoId} with CorrelationId: {CorrelationId}",
                request.Id, request.CorrelationId);

            var lancamento = await _lancamentoRepository.GetByIdAsync(request.Id);

            if (lancamento == null)
            {
                _logger.LogWarning("Lancamento with ID {LancamentoId} not found. CorrelationId: {CorrelationId}",
                    request.Id, request.CorrelationId);
                return null;
            }

            _logger.LogInformation("Lancamento with ID {LancamentoId} found. CorrelationId: {CorrelationId}",
                lancamento.Id, request.CorrelationId);

            return new LancamentoDto
            {
                Id = lancamento.Id,
                Valor = lancamento.Valor,
                Data = lancamento.Data,
                Descricao = lancamento.Descricao,
                Tipo = lancamento.Tipo.Valor,
                Categoria = lancamento.Categoria.Valor,
                Observacoes = lancamento.Observacoes,
                CorrelationId = request.CorrelationId
            };
        }
    }
}