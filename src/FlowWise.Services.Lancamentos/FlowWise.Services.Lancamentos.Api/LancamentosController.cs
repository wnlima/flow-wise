using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.Queries;
using FlowWise.Services.Lancamentos.Api.Requests;
using FlowWise.Services.Lancamentos.Api.Responses;
using FlowWise.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FlowWise.Services.Lancamentos.Api.Controllers
{
    /// <summary>
    /// Controlador para o serviço de Lançamentos.
    /// Gerencia as operações de CRUD para lançamentos financeiros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LancamentosController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="LancamentosController"/>.
        /// </summary>
        /// <param name="mediator">O mediador para enviar comandos e queries.</param>
        public LancamentosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo lançamento financeiro.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/lancamentos
        ///     {
        ///        "valor": 100.50,
        ///        "data": "2025-05-26T00:00:00Z",
        ///        "descricao": "Almoço com clientes",
        ///        "tipo": "Debito",
        ///        "categoria": "Alimentacao",
        ///        "observacoes": "Reunião de fechamento"
        ///     }
        ///
        /// REQ-FLW-LAN-001, HU-001, RN-002, RN-006, NFR-OBS-001.
        /// </remarks>
        /// <param name="request">Os dados para criação do lançamento.</param>
        /// <returns>Um <see cref="ApiResponseWithData{TData}"/> contendo o lançamento criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)] // Para erros de validação
        public async Task<IActionResult> CreateLancamento([FromBody] CreateLancamentoRequest request)
        {
            var command = new CreateLancamentoCommand(Request.HttpContext.Items["CorrelationId"]!.ToString()!)
            {
                Valor = request.Valor,
                Data = request.Data,
                Descricao = request.Descricao,
                Tipo = request.Tipo,
                Categoria = request.Categoria,
                Observacoes = request.Observacoes,
            };

            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetLancamentoById),
                                   new { id },
                                   ApiResponseWithData<Guid>.Success(id, "Lançamento criado com sucesso."));
        }

        /// <summary>
        /// Obtém um lançamento financeiro específico pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/lancamentos/{id}
        ///
        /// REQ-FLW-LAN-004, HU-001, NFR-OBS-001.
        /// </remarks>
        /// <param name="id">O identificador único do lançamento.</param>
        /// <returns>Um <see cref="ApiResponseWithData{TData}"/> contendo o lançamento, ou um erro 404 se não encontrado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<LancamentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLancamentoById([FromRoute] Guid id)
        {
            var query = new GetLancamentoByIdQuery(
                id,
                Request.HttpContext.Items["CorrelationId"]!.ToString()!
            );

            var lancamentoDto = await _mediator.Send(query); 

            if (lancamentoDto == null)
            {
                return NotFound(ApiResponse.ErrorResult($"Lançamento com ID '{id}' não encontrado.", (int)HttpStatusCode.NotFound));
            }

            var apiResponseData = new LancamentoResponse
            {
                Id = lancamentoDto.Id,
                Valor = lancamentoDto.Valor,
                Data = lancamentoDto.Data,
                Descricao = lancamentoDto.Descricao,
                Tipo = lancamentoDto.Tipo,
                Categoria = lancamentoDto.Categoria,
                Observacoes = lancamentoDto.Observacoes,
            };

            return Ok(ApiResponseWithData<LancamentoResponse>.Success(apiResponseData));
        }

        /// <summary>
        /// Obtém uma lista de lançamentos financeiros, com opção de filtros.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/lancamentos?startDate=2025-05-01&endDate=2025-05-31&tipo=Debito&categoria=Alimentacao
        ///
        /// REQ-FLW-LAN-001, HU-001, NFR-OBS-001.
        /// </remarks>
        /// <param name="startDate">Data de início para o filtro (opcional).</param>
        /// <param name="endDate">Data de fim para o filtro (opcional).</param>
        /// <param name="tipo">Tipo de lançamento para o filtro (opcional: "Credito" ou "Debito").</param>
        /// <param name="categoria">Categoria do lançamento para o filtro (opcional).</param>
        /// <returns>Um <see cref="ApiResponseWithData{TData}"/> contendo a lista de lançamentos.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<LancamentoResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLancamentos(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? tipo,
            [FromQuery] string? categoria)
        {
            var query = new GetLancamentosQuery(
                startDate?.ToUniversalTime(),
                endDate?.ToUniversalTime(),
                tipo,
                categoria,
                Request.HttpContext.Items["CorrelationId"]!.ToString()!
            );

            var lancamentosDto = await _mediator.Send(query); // Retorna IEnumerable<LancamentoDto>

            var apiResponseData = lancamentosDto.Select(dto => new LancamentoResponse
            {
                Id = dto.Id,
                Valor = dto.Valor,
                Data = dto.Data,
                Descricao = dto.Descricao,
                Tipo = dto.Tipo,
                Categoria = dto.Categoria,
                Observacoes = dto.Observacoes,
            }).ToList();

            return Ok(ApiResponseWithData<IEnumerable<LancamentoResponse>>.Success(apiResponseData));
        }

        /// <summary>
        /// Atualiza um lançamento financeiro existente.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     PUT /api/lancamentos/{id}
        ///     {
        ///        "id": "guid-do-lancamento", // Deve ser o mesmo do {id} na rota
        ///        "valor": 120.75,
        ///        "data": "2025-05-26T00:00:00Z",
        ///        "descricao": "Almoço atualizado",
        ///        // "tipo": "Debito", // Tipo não pode ser alterado (RN-005)
        ///        "categoria": "Refeicao",
        ///        "observacoes": "Reunião de fechamento e novas diretrizes"
        ///     }
        ///
        /// REQ-FLW-LAN-003, HU-004, RN-001, RN-005, RN-006, NFR-OBS-001.
        /// </remarks>
        /// <param name="id">O identificador único do lançamento a ser atualizado.</param>
        /// <param name="request">Os dados para atualização do lançamento. O Command <see cref="UpdateLancamentoCommand"/> já possui o Id.</param>
        /// <returns>Um <see cref="ApiResponse"/> indicando sucesso ou erro.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)] // Para IDs divergentes ou erros de validação
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)] // Se o lançamento não for encontrado
        public async Task<IActionResult> UpdateLancamento([FromRoute] Guid id, [FromBody] UpdateLancamentoRequest request)
        {
            var command = new UpdateLancamentoCommand(Request.HttpContext.Items["CorrelationId"]!.ToString()!)
            {
                Id = id,
                Valor = request.Valor,
                Data = request.Data,
                Descricao = request.Descricao,
                Tipo = request.Tipo,
                Categoria = request.Categoria,
                Observacoes = request.Observacoes,
            };

            await _mediator.Send(command);

            return Ok(ApiResponse.SuccessResult("Lançamento atualizado com sucesso."));
        }

        /// <summary>
        /// Exclui logicamente um lançamento financeiro pelo seu ID.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     DELETE /api/lancamentos/{id}
        ///
        /// REQ-FLW-LAN-003, HU-004, RN-001, NFR-OBS-001.
        /// </remarks>
        /// <param name="id">O identificador único do lançamento a ser excluído.</param>
        /// <returns>Um <see cref="ApiResponse"/> indicando sucesso (NoContent) ou erro.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)] // ApiResponse implícito via middleware para erros
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLancamento([FromRoute] Guid id)
        {
            var command = new DeleteLancamentoCommand(Request.HttpContext.Items["CorrelationId"]!.ToString()!)
            {
                Id = id
            };

            await _mediator.Send(command);

            return NoContent();
        }
    }
}