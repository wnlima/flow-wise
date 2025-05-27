using MediatR;
using FlowWise.Services.Consolidacao.Application.Queries;
using FlowWise.Services.Consolidacao.Application.Dtos;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Consolidacao.Application.QueryHandlers;

/// <summary>
/// [CQRS] Query Handler: Lida com a query <see cref="GetSaldoDiarioQuery"/>.
/// Responsável por buscar o saldo diário consolidado.
/// </summary>
/// <remarks>
/// [HU-006]: Responde pela funcionalidade de visualização do saldo diário consolidado (D-1).
/// [NFR-PERF-002]: Otimizado para performance em cenários de alta concorrência (50 requisições/segundo).
/// [NFR-OBS-001]: Garante que o Correlation ID seja propagado e usado no log.
/// [DDD]: Interage com o repositório de domínio.
/// </remarks>
public class GetSaldoDiarioQueryHandler : IRequestHandler<GetSaldoDiarioQuery, SaldoDiarioDto?>
{
    private readonly ISaldoDiarioRepository _saldoDiarioRepository;
    private readonly ILogger<GetSaldoDiarioQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="GetSaldoDiarioQueryHandler"/>.
    /// </summary>
    /// <param name="saldoDiarioRepository">O repositório de saldo diário para buscar os dados.</param>
    /// <param name="logger">O logger para registrar informações.</param>
    public GetSaldoDiarioQueryHandler(ISaldoDiarioRepository saldoDiarioRepository, ILogger<GetSaldoDiarioQueryHandler> logger)
    {
        _saldoDiarioRepository = saldoDiarioRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lida com a requisição para obter o saldo diário consolidado.
    /// </summary>
    /// <param name="request">A query <see cref="GetSaldoDiarioQuery"/>.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Um <see cref="SaldoDiarioDto"/> contendo o saldo consolidado, ou null se não encontrado.</returns>
    public async Task<SaldoDiarioDto?> Handle(GetSaldoDiarioQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetSaldoDiarioQuery for date {Data} with CorrelationId: {CorrelationId}",
            request.Data.ToShortDateString(), request.CorrelationId);

        var saldoDiario = await _saldoDiarioRepository.GetByDateAsync(request.Data);

        if (saldoDiario == null)
        {
            _logger.LogWarning("SaldoDiario for date {Data} not found. CorrelationId: {CorrelationId}",
                request.Data.ToShortDateString(), request.CorrelationId);
            return null;
        }

        _logger.LogInformation("SaldoDiario for date {Data} found. Saldo Total: {SaldoTotal}. CorrelationId: {CorrelationId}",
            saldoDiario.Data.ToShortDateString(), saldoDiario.SaldoTotal, request.CorrelationId);

        return new SaldoDiarioDto
        {
            Data = saldoDiario.Data,
            SaldoTotal = saldoDiario.SaldoTotal,
            TotalCreditos = saldoDiario.TotalCreditos,
            TotalDebitos = saldoDiario.TotalDebitos,
            UltimaAtualizacao = saldoDiario.UltimaAtualizacao
        };
    }
}
