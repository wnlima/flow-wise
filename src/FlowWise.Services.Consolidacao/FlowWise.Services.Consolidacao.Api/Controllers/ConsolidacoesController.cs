using FlowWise.Common;
using FlowWise.Services.Consolidacao.Api.Responses;
using FlowWise.Services.Consolidacao.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlowWise.Services.Consolidacao.Api.Controllers
{
    /// <summary>
    /// Controlador para o serviço de Consolidação.
    /// Gerencia as operações de consulta de saldos diários e relatórios de fluxo de caixa.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConsolidacoesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ConsolidacoesController"/>.
        /// </summary>
        /// <param name="mediator">O mediador para enviar queries.</param>
        public ConsolidacoesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtém o saldo diário consolidado para uma data específica.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/consolidados?data=2025-05-22
        ///
        /// [HU-006]: Permite a visualização do saldo diário consolidado (D-1).
        /// [RN-003]: O saldo consolidado é sempre de D-1 (dia anterior).
        /// [NFR-PERF-002]: Otimizado para suportar 50 requisições por segundo.
        /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado.
        /// </remarks>
        /// <param name="data">A data para a qual o saldo consolidado é solicitado. Deve ser uma data no passado ou o dia atual.</param>
        /// <returns>Um <see cref="ApiResponseWithData{TData}"/> contendo o saldo diário, ou um erro 404 se não encontrado.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<SaldoDiarioResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaldoDiario([FromQuery] DateTime data)
        {
            // Embora já validado na entidade SaldoDiario.Create, validar aqui para feedback precoce.
            if (data.Date > DateTime.Today.Date)
            {
                return BadRequest(ApiResponse.ErrorResult("A data para consulta do saldo diário não pode ser futura.", (int)HttpStatusCode.BadRequest, correlationId: Request.HttpContext.Items["CorrelationId"]?.ToString()));
            }

            var query = new GetSaldoDiarioQuery(data, Request.HttpContext.Items["CorrelationId"]?.ToString());
            var saldoDiarioDto = await _mediator.Send(query);

            if (saldoDiarioDto == null)
            {
                return NotFound(ApiResponse.ErrorResult($"Saldo diário para a data '{data.ToShortDateString()}' não encontrado.", (int)HttpStatusCode.NotFound, correlationId: Request.HttpContext.Items["CorrelationId"]?.ToString()));
            }

            var apiResponseData = new SaldoDiarioResponse
            {
                Data = saldoDiarioDto.Data,
                SaldoTotal = saldoDiarioDto.SaldoTotal,
                TotalCreditos = saldoDiarioDto.TotalCreditos,
                TotalDebitos = saldoDiarioDto.TotalDebitos,
                UltimaAtualizacao = saldoDiarioDto.UltimaAtualizacao
            };

            return Ok(ApiResponseWithData<SaldoDiarioResponse>.Success(apiResponseData));
        }

        /// <summary>
        /// Gera um relatório de fluxo de caixa para um período específico.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/consolidados/relatorio?dataInicio=2025-05-01&dataFim=2025-05-31
        ///
        /// [HU-007]: Permite a geração de relatórios de fluxo de caixa por período.
        /// [NFR-PERF-004]: Otimizado para tempo de geração de relatório (máximo 5 segundos).
        /// [NFR-OBS-001]: Garante que o Correlation ID seja propagado.
        /// </remarks>
        /// <param name="dataInicio">A data de início do período (formato YYYY-MM-DD).</param>
        /// <param name="dataFim">A data de fim do período (formato YYYY-MM-DD).</param>
        /// <returns>Um <see cref="ApiResponseWithData{TData}"/> contendo o relatório de fluxo de caixa.</returns>
        [HttpGet("relatorio")]
        [ProducesResponseType(typeof(ApiResponseWithData<RelatorioFluxoCaixaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRelatorioFluxoCaixa(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            if (dataInicio.Date > dataFim.Date)
            {
                return BadRequest(ApiResponse.ErrorResult("A data de início não pode ser posterior à data de fim.", (int)HttpStatusCode.BadRequest, correlationId: Request.HttpContext.Items["CorrelationId"]?.ToString()));
            }
            if (dataFim.Date > DateTime.Today.Date)
            {
                return BadRequest(ApiResponse.ErrorResult("A data de fim do relatório não pode ser futura.", (int)HttpStatusCode.BadRequest, correlationId: Request.HttpContext.Items["CorrelationId"]?.ToString()));
            }
            // [HU-007] Critério de Aceitação: máximo 30 dias de período inicial
            if ((dataFim.Date - dataInicio.Date).TotalDays > 30)
            {
                return BadRequest(ApiResponse.ErrorResult("O período do relatório não pode exceder 30 dias.", (int)HttpStatusCode.BadRequest, correlationId: Request.HttpContext.Items["CorrelationId"]?.ToString()));
            }

            var query = new GetRelatorioFluxoCaixaQuery(dataInicio, dataFim, Request.HttpContext.Items["CorrelationId"]?.ToString());
            var relatorioDto = await _mediator.Send(query);

            var apiResponseData = new RelatorioFluxoCaixaResponse
            {
                DataInicio = relatorioDto.DataInicio,
                DataFim = relatorioDto.DataFim,
                SaldoInicial = relatorioDto.SaldoInicial,
                TotalCreditos = relatorioDto.TotalCreditos,
                TotalDebitos = relatorioDto.TotalDebitos,
                SaldoFinal = relatorioDto.SaldoFinal
            };

            return Ok(ApiResponseWithData<RelatorioFluxoCaixaResponse>.Success(apiResponseData));
        }
    }
}