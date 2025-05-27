using MediatR;
using FlowWise.Services.Consolidacao.Application.Queries;
using FlowWise.Services.Consolidacao.Application.Dtos;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowWise.Services.Consolidacao.Application.QueryHandlers;

/// <summary>
/// [CQRS] Query Handler: Lida com a query <see cref="GetRelatorioFluxoCaixaQuery"/>.
/// Responsável por gerar um relatório de fluxo de caixa consolidado para um período específico.
/// </summary>
/// <remarks>
/// [HU-007]: Responde pela funcionalidade de geração de relatórios de fluxo de caixa por período.
/// [NFR-PERF-004]: Otimizado para performance na geração do relatório.
/// [NFR-OBS-001]: Garante que o Correlation ID seja propagado e usado no log.
/// [DDD]: Interage com o repositório de domínio para buscar os saldos diários.
/// </remarks>
public class GetRelatorioFluxoCaixaQueryHandler : IRequestHandler<GetRelatorioFluxoCaixaQuery, RelatorioFluxoCaixaDto>
{
    private readonly ISaldoDiarioRepository _saldoDiarioRepository;
    private readonly ILogger<GetRelatorioFluxoCaixaQueryHandler> _logger;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="GetRelatorioFluxoCaixaQueryHandler"/>.
    /// </summary>
    /// <param name="saldoDiarioRepository">O repositório de saldo diário para buscar os dados.</param>
    /// <param name="logger">O logger para registrar informações.</param>
    public GetRelatorioFluxoCaixaQueryHandler(ISaldoDiarioRepository saldoDiarioRepository, ILogger<GetRelatorioFluxoCaixaQueryHandler> logger)
    {
        _saldoDiarioRepository = saldoDiarioRepository;
        _logger = logger;
    }

    /// <summary>
    /// Lida com a requisição para gerar o relatório de fluxo de caixa.
    /// </summary>
    /// <param name="request">A query <see cref="GetRelatorioFluxoCaixaQuery"/>.</param>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>Um <see cref="RelatorioFluxoCaixaDto"/> contendo o relatório consolidado.</returns>
    public async Task<RelatorioFluxoCaixaDto> Handle(GetRelatorioFluxoCaixaQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetRelatorioFluxoCaixaQuery for period {DataInicio} to {DataFim} with CorrelationId: {CorrelationId}",
            request.DataInicio.ToShortDateString(), request.DataFim.ToShortDateString(), request.CorrelationId);


        // Obter todos os saldos diários no período (incluindo a data de início e fim)
        var saldosNoPeriodo = (await _saldoDiarioRepository.GetByDateRangeAsync(request.DataInicio, request.DataFim))
                                .OrderBy(sd => sd.Data)
                                .ToList();

        decimal saldoInicial = 0;
        decimal totalCreditos = 0;
        decimal totalDebitos = 0;
        decimal saldoFinal = 0;

        if (saldosNoPeriodo.Any())
        {
            // Para o saldo inicial, precisamos do saldo final do dia ANTERIOR à DataInicio
            // (se houver, caso contrário, começa em zero).
            var diaAnteriorAoInicio = request.DataInicio.AddDays(-1).Date;
            var saldoDiaAnterior = await _saldoDiarioRepository.GetByDateAsync(diaAnteriorAoInicio);
            saldoInicial = saldoDiaAnterior?.SaldoTotal ?? 0;

            totalCreditos = saldosNoPeriodo.Sum(sd => sd.TotalCreditos);
            totalDebitos = saldosNoPeriodo.Sum(sd => sd.TotalDebitos);
            saldoFinal = saldoInicial + (totalCreditos - totalDebitos); // Saldo final é SaldoInicial + (Créditos - Débitos do Período)
        }
        else
        {
            _logger.LogWarning("Nenhum saldo diário encontrado para o período {DataInicio} a {DataFim}. Retornando relatório com valores zerados. CorrelationId: {CorrelationId}",
                request.DataInicio.ToShortDateString(), request.DataFim.ToShortDateString(), request.CorrelationId);
        }

        _logger.LogInformation("Relatório de Fluxo de Caixa gerado para o período {DataInicio} a {DataFim}. Saldo Final: {SaldoFinal}. CorrelationId: {CorrelationId}",
            request.DataInicio.ToShortDateString(), request.DataFim.ToShortDateString(), saldoFinal, request.CorrelationId);

        return new RelatorioFluxoCaixaDto
        {
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            SaldoInicial = saldoInicial,
            TotalCreditos = totalCreditos,
            TotalDebitos = totalDebitos,
            SaldoFinal = saldoFinal
        };
    }
}